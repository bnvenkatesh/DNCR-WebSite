using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SD.ACMA.BusinessLogic.PaymentGateway;
using SD.ACMA.DatabaseIntermediary;
using SD.ACMA.DNCR.Infrastructure;
using SD.ACMA.DNCR.Infrastructure.Configuration;
using SD.ACMA.DNCRProject.Website.Helpers;
using SD.ACMA.DNCRProject.Website.Models;
using SD.ACMA.InterfaceTier;
using SD.ACMA.POCO.Industry;
using SD.Core.Data.Repository.PetaPoco.Business;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using SubscriptionModel = SD.ACMA.DNCRProject.Website.Models.SubscriptionModel;

namespace SD.ACMA.DNCRProject.Website.Controllers
{
    public class SubscriptionSurfaceController : SurfaceController
    {
        private readonly IIndustryDataInterchange _industryDataInterchange;
        private readonly IErrorMessageHelper _errorMessageHelper;
        private readonly IUserContextHelper _userContextHelper;
        private readonly ICreditCardPaymentService _creditCardPaymentService;
        private readonly IPaymentGatewayService _paymentGatewayService;

        readonly string SubscriptionDateParseExactFormat = ConfigurationHelper.Instance.AccountSummary_SubscriptionDate_ParseExactFormat;

        public SubscriptionSurfaceController(IIndustryDataInterchange industryDataInterchange, IErrorMessageHelper errorMessageHelper, IUserContextHelper userContextHelper, ICreditCardPaymentService creditCardPaymentService, IPaymentGatewayService paymentGatewayService)
        {
            _industryDataInterchange = industryDataInterchange;
            _errorMessageHelper = errorMessageHelper;
            _userContextHelper = userContextHelper;
            _creditCardPaymentService = creditCardPaymentService;
            _paymentGatewayService = paymentGatewayService;
        }

        #region Purchase Subscription

        [ChildActionOnly]
        public ActionResult Purchase()
        {
            ViewBag.PageTitle = CurrentPage.GetPropertyValue("pageTitle");
            ViewBag.PageSummary = CurrentPage.GetPropertyValue("pageSummary");
            ViewBag.Faqs = CurrentPage.GetPropertyValue("faqs");
            ViewBag.Downloads = CurrentPage.GetPropertyValue("downloads");
            ViewBag.RelatedLinks = CurrentPage.GetPropertyValue("relatedLinks").ToString();
            var home = CurrentPage.AncestorOrSelf(1);
            var paymentLimit = home.GetPropertyValue("creditCardPaymentLimit");
            ViewBag.PaymentLimit = paymentLimit ?? 30000;
            IPublishedContent industryLodgeEnquiryNode = Umbraco.Content(home.GetPropertyValue("industryLodgeEnquiryNode"));
            ViewBag.IndustryLodgeEnquiryUrl = industryLodgeEnquiryNode.Url;

            var accountId = SessionHelper.AccountId;
            var getSubscriptionSummaryDetailsResult = _industryDataInterchange.GetSubscriptionSummaryDetailsByAccountID(accountId);

            var model = new SubscriptionViewModel(){IsSubmitted = false, UseAccountBalance = true};

            ViewBag.AccountBalance = getSubscriptionSummaryDetailsResult != null
                ? getSubscriptionSummaryDetailsResult.SubscriptionSummary.CreditNotes
                : 0;
            model.SubscriptionModels = GetSubscriptionModels(accountId);

            var accountUserId = SessionHelper.AccountUserId;
            if (accountUserId > 0)
            {
                var ucm = _userContextHelper.CreateUserContextObject(accountUserId, SessionHelper.AgentId);

                var userResult = _industryDataInterchange.GetAccountUser(accountUserId, ucm);
                if (userResult != null)
                {
                    //model.Name = userResult.FirstName + " " + userResult.LastName;
                    model.Email = userResult.EmailAddress;
                    model.ConfirmEmail = userResult.EmailAddress;
                }
            }

            return PartialView("Purchase", model);
        }

        [NotChildAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Purchase(SubscriptionViewModel model)
        {
            ModelState.Remove("MinimumSubscriptionModels");

            if (model.PaymentDue == 0)
            {
                ModelState.Remove("PaymentMethod");
            }

            var confirmationUrl = CurrentPage.Children.Any() ? CurrentPage.Children.First().Url : null;
            var host = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority;

            if (ModelState.IsValid && !String.IsNullOrEmpty(confirmationUrl) && !String.IsNullOrEmpty(host))
            {
                var accountId = SessionHelper.AccountId;
                var accountUserId = SessionHelper.AccountUserId;
                int? agentId = SessionHelper.AgentId;
                var ucm = _userContextHelper.CreateUserContextObject(accountUserId, agentId);

                var order = new List<SubscriptionVsQuantityModel>();
                model.IsFreeSubscription = false;
                foreach (var subscription in model.SubscriptionModels)
                {
                    if (subscription.Quantity > 0)
                    {
                        order.Add(new SubscriptionVsQuantityModel(){SubscriptionID = subscription.Id, Quantity = subscription.Quantity});
                        if (subscription.Type == "A")
                        {
                            model.IsFreeSubscription = true;
                        }
                    }
                }

                if (model.AccountBalanceToUse == 0)
                {
                    model.UseAccountBalance = false;
                }
                var result = _industryDataInterchange.CreateOrder(accountId, model.AccountBalanceToUse, model.Email, model.UseAccountBalance, model.PaymentMethod == "Credit card", order, ucm);

                if (result.Errors == null)
                {
                    ViewBag.OrderId = result.OrderId;
                    //pay for free subscription order
                    if (model.PaymentDue == 0 && !model.UseAccountBalance)
                    {
                        var args = new PayForOrderArguments()
                                   {
                                       AccountUserID = accountUserId,
                                       AgentUserID = agentId,
                                       Amount = 0,
                                       AuthorizeID = null,
                                       CardType = null,
                                       CreditCardReference = null,
                                       ReceiptNo = null,
                                       IsCSR = agentId != null,
                                       OrderID = result.OrderId,
                                       ResponseCode = null,
                                       TransactionID = null,
                                       TransactionNo = 0,
                                       TransactionType = null
                                   };
                        var payResult = _industryDataInterchange.PayForOrder(args);

                        if (!payResult.IsSuccessful || payResult.Errors != null)
                        {
                            ViewBag.ErrorMessage = payResult.Errors.Message;
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(payResult.EnquiryId))
                            {
                                model.EnquiryId = payResult.EnquiryId;
                                SessionHelper.RequireLogout = true;
                            }

                            model.IsSubmitted = true;
                        }
                    }
                    else
                    {
                        model.IsSubmitted = true;

                        if (model.PaymentMethod == "Credit card")
                        {
                            //Create Payment
                            var orderNumber = result.OrderNumber;
                            var orderId = result.OrderId;
                            string transactionId;
                            if (
                                _creditCardPaymentService.CreatePayment(orderId, orderNumber, accountId,
                                    (int) (model.PaymentDue*100), out transactionId) &&
                                !String.IsNullOrEmpty(transactionId))
                            {
                                //Redirect Access Seeker
                                var url = _paymentGatewayService.GetPaymentGatewayUrl(transactionId, orderNumber,
                                    (int) (model.PaymentDue*100), host + confirmationUrl);
                                if (!String.IsNullOrEmpty(url))
                                    return new RedirectResult(url);
                            }
                        }
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = result.Errors.Message;
                }
            }
            TempData.Add("SubscriptionViewModel", model);
            return CurrentUmbracoPage();
        }

        /*private string GetOrderInfo(List<SubscriptionModel> subscriptions)
        {
            var sb = new StringBuilder();
            foreach (var subscription in subscriptions)
            {
                if (subscription.Quantity > 0)
                {
                    sb.Append(String.Format("{0} x Subscription {1}, ", subscription.Quantity, subscription.Type));
                }
            }
            return sb.ToString().Trim().TrimEnd(',');
        }*/

        private List<SubscriptionModel> GetSubscriptionModels(int accountId) 
        {
            //TO DO: Get these model from Avanade
            var subscriptionModels = new List<SubscriptionModel>();
            //*
            var result = _industryDataInterchange.GetAvailableSubscriptions(accountId);
            if (result != null)
            {
                foreach (var subscription in result.Subscriptions)
                {
                    subscriptionModels.Add(new SubscriptionModel
                                           {
                                               Id = subscription.SubscriptionRequestID,
                                               Type = subscription.SubscriptionName,
                                               Limit = subscription.WashCredits,
                                               Price = subscription.Price,
                                               IsFree = subscription.IsFreeSubscription
                                           });
                }
            }
            //*/
            /*
            var s1 = new SubscriptionModel { Id = 1, Type = "A", Limit = 500, Price = 0, IsFree = true };
            var s2 = new SubscriptionModel { Id = 2, Type = "B", Limit = 20000, Price = 79 };
            var s3 = new SubscriptionModel { Id = 3, Type = "C", Limit = 100000, Price = 370 };
            var s4 = new SubscriptionModel { Id = 4, Type = "D", Limit = 1000000, Price = 3200 };
            var s5 = new SubscriptionModel { Id = 5, Type = "E", Limit = 10000000, Price = 27000 };
            var s6 = new SubscriptionModel { Id = 6, Type = "F", Limit = 20000000, Price = 45000 };
            var s7 = new SubscriptionModel { Id = 7, Type = "G", Limit = 50000000, Price = 67500 };
            var s8 = new SubscriptionModel { Id = 8, Type = "H", Limit = 100000000, Price = 90000 };

            subscriptionModels.Add(s1);
            subscriptionModels.Add(s2);
            subscriptionModels.Add(s3);
            subscriptionModels.Add(s4);
            subscriptionModels.Add(s5);
            subscriptionModels.Add(s6);
            subscriptionModels.Add(s7);
            subscriptionModels.Add(s8);
            //*/
            return subscriptionModels;
        }

        [ChildActionOnly]
        public ActionResult Confirmation(string receipt)
        {
            var home = CurrentPage.AncestorOrSelf(1);
            IPublishedContent accountDashboardNode = Umbraco.Content(home.GetPropertyValue("accountDashboardNode"));
            IPublishedContent makePaymentNode = Umbraco.Content(home.GetPropertyValue("makePaymentNode"));

            ViewBag.MakePaymentUrl = makePaymentNode.Url;


            var accountUserId = SessionHelper.AccountUserId;
            int? agentId = SessionHelper.AgentId;

            ViewBag.IsSuccess = false;
            //Access Seeker return
            if (!string.IsNullOrEmpty(receipt))
            {
                //VerifyReceipt
                string transactionId;
                Enums.PaymentStatusEnum paymentStatus;
                string response;
                string message;
                string creditCardReference;
                string receiptNumber;
                DateTime? settlementDate;
                long? transactionNo;
                int? transactionAmountInCents;
                string authorizeId;
                string transactionType;
                string cardType;
                _paymentGatewayService.VerifyReceipt(receipt, out transactionId, out paymentStatus, out response, out message, out creditCardReference, out receiptNumber, out settlementDate, out transactionNo, out transactionAmountInCents, out authorizeId, out transactionType, out cardType);

                //check if is has been processed
                var payment = _creditCardPaymentService.GetCreditCardPayment(transactionId);

                if (!payment.IsProcessed)
                {
                    int? orderId;
                    //Update Payment Status
                    _creditCardPaymentService.UpdateStatus(transactionId, receipt, response, message, receiptNumber,
                        settlementDate, paymentStatus, creditCardReference, transactionNo, transactionAmountInCents,
                        authorizeId, transactionType, cardType, out orderId);

                    if (paymentStatus == Enums.PaymentStatusEnum.SUCCESS)
                    {
                        if (orderId != null)
                        {
                            var args = new PayForOrderArguments()
                                       {
                                           AccountUserID = accountUserId,
                                           AgentUserID = agentId ?? 0,
                                           Amount = transactionAmountInCents/100 ?? 0,
                                           AuthorizeID = authorizeId,
                                           CardType = cardType,
                                           CreditCardReference = creditCardReference,
                                           ReceiptNo = receiptNumber,
                                           IsCSR = agentId != null,
                                           OrderID = orderId.Value,
                                           ResponseCode = response,
                                           TransactionID = transactionId,
                                           TransactionNo = transactionNo.Value,
                                           TransactionType = transactionType,
                                           ExternalMessage = message,
                                           ExpectedSettelmentDate = settlementDate,
                                       };
                            var result = _industryDataInterchange.PayForOrder(args);

                            if (result.IsSuccessful || (!result.IsSuccessful && result.Errors != null))
                                _creditCardPaymentService.SetTransactionAsProcessed(transactionId, result.IsSuccessful,
                                    result.Errors != null ? result.Errors.Message : null);
                        }

                        ViewBag.IsSuccess = true;
                        ViewBag.OrderId = orderId;
                    }
                    else if (paymentStatus == Enums.PaymentStatusEnum.DECLINED)
                    {
                        if (orderId != null)
                        {
                            var args = new PayForOrderArguments()
                                       {
                                           AccountUserID = accountUserId,
                                           AgentUserID = agentId ?? 0,
                                           Amount = transactionAmountInCents/100 ?? 0,
                                           AuthorizeID = authorizeId,
                                           CardType = cardType,
                                           CreditCardReference = creditCardReference,
                                           ReceiptNo = receiptNumber,
                                           IsCSR = agentId != null,
                                           OrderID = orderId.Value,
                                           ResponseCode = response,
                                           TransactionID = transactionId,
                                           TransactionNo = transactionNo.Value,
                                           TransactionType = transactionType,
                                           ExternalMessage = message,
                                           ExpectedSettelmentDate = settlementDate,
                                       };
                            _industryDataInterchange.RecordPayForOrderFail(args);
                        }
                        ViewBag.OrderId = orderId;
                    }
                }
                else
                {
                    ControllerContext.HttpContext.Response.Redirect(accountDashboardNode.Url);
                }
            }
            return PartialView("Confirmation");
        }

        #endregion

        #region Cancel Order

        [ChildActionOnly]
        public ActionResult CancelOrder(int paymentLimit, string makePaymentUrl)
        {
            ViewBag.PaymentLimit = paymentLimit;
            ViewBag.MakePaymentUrl = makePaymentUrl;

            return PartialView("CancelOrder");
        }

        [NotChildAction]
        public ActionResult CancelOrder(int orderId, int currentPage, int paymentLimit, string makePaymentUrl)
        {
            var accountUserId = SessionHelper.AccountUserId;
            int? agentId = SessionHelper.AgentId;
            var ucm = _userContextHelper.CreateUserContextObject(accountUserId, agentId);

            var result = _industryDataInterchange.CancelOrder(accountUserId, orderId, agentId, agentId != null ? true : false);

            if (result.Errors != null)
            {
                ViewBag.ErrorMessage = _errorMessageHelper.GenerateErrorMessage(result.Errors);
            }
            else if (!result.IsSuccessful)
            {
                ViewBag.ErrorMessage = result.Message;
            }

            var model = getViewModel(currentPage, paymentLimit, makePaymentUrl);
            return PartialView("FinancialHistory", model);
        }

        #endregion

        #region Close Order

        [ChildActionOnly]
        public ActionResult CloseOrder(int paymentLimit, string makePaymentUrl )
        {
            ViewBag.PaymentLimit = paymentLimit;
            ViewBag.MakePaymentUrl = makePaymentUrl;

            return PartialView("CloseOrder");
        }

        [NotChildAction]
        public ActionResult CloseOrder(int orderId, int currentPage, int paymentLimit, string makePaymentUrl)
        {
            var accountUserId = SessionHelper.AccountUserId;
            int? agentId = SessionHelper.AgentId;
            var ucm = _userContextHelper.CreateUserContextObject(accountUserId, agentId);
            

            var result = _industryDataInterchange.CloseOrder(accountUserId, orderId, agentId, agentId != null ? true : false);

            if (result.Errors != null)
            {
                ViewBag.ErrorMessage = _errorMessageHelper.GenerateErrorMessage(result.Errors);
            }
            else if (!result.IsSuccessful)
            {
                ViewBag.ErrorMessage = result.Message;
            }

            var model = getViewModel(currentPage, paymentLimit, makePaymentUrl);
            return PartialView("FinancialHistory", model);
        }

        #endregion

        #region Subscription History

        public ActionResult Dashboard(int currentPage)
        {
            var itemsPerPage = 10;
            var model = new DashboardSubscriptionViewModel();
            var subscriptions = GetAllSubscription();
            var pager = model.Pager = PagerHelper.GetPager(itemsPerPage, subscriptions.Count(), currentPage);
            model.Subscriptions = subscriptions.Skip((pager.CurrentPage - 1) * pager.ItemsPerPage).Take(pager.ItemsPerPage);
            return PartialView("Dashboard", model);
        }

        private IList<DashboardSubscriptionModel> GetAllSubscription()
        {
            var models = new List<DashboardSubscriptionModel>();

            var result = _industryDataInterchange.GetAccountSummaryDetailByAccountID(SessionHelper.AccountId);

            if (result.Errors != null)
            {
                //TODO: How do we handle errors?
            }
            else
            {
                if (result.AccountSummaries != null && result.AccountSummaries.Count > 0)
                {
                    foreach (var item in result.AccountSummaries)
                    {
                        var dsm = new DashboardSubscriptionModel
                        {
                            Date = DateTime.ParseExact(item.CreatedTimeStampt, SubscriptionDateParseExactFormat, null).ToString("dd MMM yyyy"),
                            Transaction = item.TransactionType,
                            OrderNumber = item.OrderNumber,
                            Description = item.Description,
                            Amount = item.Amount,
                            WashBalance = item.WashCredits,
                        };

                        models.Add(dsm);
                    }
                }
            }
            
            return models;
        }

        #endregion

        #region Financial History

        public ActionResult FinancialHistory(int currentPage, int paymentLimit, string makePaymentUrl)
        {
            var model = getViewModel(currentPage, paymentLimit, makePaymentUrl);

            return PartialView("FinancialHistory", model);
        }

        private DashboardFinancialViewModel getViewModel(int currentPage, int paymentLimit, string makePaymentUrl)
        {
            var itemsPerPage = 10;
            var model = new DashboardFinancialViewModel();
            var financials = GetAllFinancial(paymentLimit).OrderByDescending(item => item.OrderNumber);
            var pager = model.Pager = PagerHelper.GetPager(itemsPerPage, financials.Count(), currentPage);
            model.Financials = financials.Skip((pager.CurrentPage - 1) * pager.ItemsPerPage).Take(pager.ItemsPerPage);
            ViewBag.PaymentLimit = paymentLimit;
            ViewBag.MakePaymentUrl = makePaymentUrl;
            return model;
        }

        private IList<DashboardFinancialModel> GetAllFinancial(int paymentLimit)
        {
            var models = new List<DashboardFinancialModel>();
            var accountId = SessionHelper.AccountId;

            var result = _industryDataInterchange.GetFinancialHistory(accountId, null);

            if (result.Errors != null)
            {
                //TODO: How do we handle errors?
            }
            else
            {
                if (result.Orders != null && result.Orders.Count > 0)
                {
                    var pendingOrders = new List<int>();

                    var pendingPayments = _creditCardPaymentService.GetPendingPayments(accountId);
                    foreach (var pendingPayment in pendingPayments)
                    {
                        var transactionId = pendingPayment.TransactionId;

                        string receipt;
                        Enums.PaymentStatusEnum paymentStatus;
                        string response;
                        string message;
                        string creditCardReference;
                        string receiptNumber;
                        DateTime? settlementDate;
                        long? transactionNo;
                        int? transactionAmountInCents;
                        string authorizeId;
                        string transactionType;
                        string cardType;
                        int? orderId;

                        _paymentGatewayService.QueryPayment(transactionId, out receipt, out paymentStatus, out response, out message, out creditCardReference, out receiptNumber, out settlementDate, out transactionNo, out transactionAmountInCents, out authorizeId, out transactionType, out cardType);

                        _creditCardPaymentService.UpdateStatus(transactionId, receipt, response, message, receiptNumber,
                            settlementDate, paymentStatus, creditCardReference, transactionNo, transactionAmountInCents,
                            authorizeId, transactionType, cardType, out orderId);
                    }

                    var updateablePayments = _creditCardPaymentService.GetUpdatable(accountId);

                    foreach (var updateablePayment in updateablePayments)
                    {
                        pendingOrders.Add(updateablePayment.OrderId);
                    }

                    foreach (var item in result.Orders)
                    {
                        var orderExpires = String.Empty;
                        if (item.OrderStatus == Enums.OrderStatusEnum.NotPaid || item.OrderStatus == Enums.OrderStatusEnum.PartPaid)
                        {
                            if (item.OrderExpiryDate.Date >= DateTime.Now.Date)
                            {
                                orderExpires =
                                    String.Format(
                                        item.OrderExpiryDate.Date.Subtract(DateTime.Now.Date).Days == 1
                                            ? "{0} day"
                                            : "{0} days", item.OrderExpiryDate.Subtract(DateTime.Now).Days);
                            }
                            else orderExpires = "Order Expired";
                        }
                        else if (item.OrderStatus == Enums.OrderStatusEnum.Cancelled)
                        {
                            orderExpires = "Order Cancelled";
                        }
                        else if (item.OrderStatus == Enums.OrderStatusEnum.Closed)
                        {
                            orderExpires = "Order Closed";
                        }
                        else if (item.OrderStatus == Enums.OrderStatusEnum.FullyPaid)
                        {
                            orderExpires = "Order Completed";
                        }
                        else if (item.OrderStatus == Enums.OrderStatusEnum.PartiallyRefunded ||
                                 item.OrderStatus == Enums.OrderStatusEnum.Refunded)
                        {
                            orderExpires = "Order Refunded";
                        }
                        else orderExpires = "Order Expired";

                        var dfm = new DashboardFinancialModel
                        {
                            OrderDate = item.CreatedTimeStamp,
                            OrderAmount = item.Amount,
                            OrderNumber = item.OrderNumber,
                            OrderId = item.OrderID,
                            Status = pendingOrders.Contains(item.OrderID) ? "Pending" : item.OrderStatus.ToString(),
                            OrderExpires = orderExpires,
                            Outstanding = item.PaymentOutstanding,
                            CanPay = OrderIsPayable(paymentLimit, pendingOrders, item, orderExpires),
                            CanCancel = item.OrderStatus == Enums.OrderStatusEnum.NotPaid && !pendingOrders.Contains(item.OrderID) && orderExpires != "Order Expired",
                            CanClose = item.OrderStatus == Enums.OrderStatusEnum.PartPaid && !pendingOrders.Contains(item.OrderID) && orderExpires != "Order Expired",
                            CanDownload = item.OrderStatus != Enums.OrderStatusEnum.Cancelled,
                        };

                        models.Add(dfm);
                    }
                }
            }
            
            return models;
        }

        private static bool OrderIsPayable(int paymentLimit, List<int> pendingOrders, OrderModel item, string orderExpires)
        {
            return (item.OrderStatus == Enums.OrderStatusEnum.NotPaid || item.OrderStatus == Enums.OrderStatusEnum.PartPaid)
                                            && !pendingOrders.Contains(item.OrderID) && item.PaymentOutstanding <= paymentLimit && item.PaymentOutstanding > 0 && orderExpires != "Order Expired";
        }

        private static bool OrderIsPayable(OrderModel item)
        {
            return (item.OrderStatus == Enums.OrderStatusEnum.NotPaid || item.OrderStatus == Enums.OrderStatusEnum.PartPaid)
                                            && item.PaymentOutstanding > 0 && item.OrderExpiryDate > DateTime.Now;
        }


        #endregion

        #region Make Payment

        [ChildActionOnly]
        public ActionResult MakePayment(int? orderId)
        {
            var home = CurrentPage.AncestorOrSelf(1);
            IPublishedContent accountDashboardNode = Umbraco.Content(home.GetPropertyValue("accountDashboardNode"));

            if (orderId == null)
            {
                ControllerContext.HttpContext.Response.Redirect(accountDashboardNode.Url);
                return null;
            }

            ViewBag.AccountDashboardURL = accountDashboardNode.Url;
            ViewBag.PageTitle = CurrentPage.GetPropertyValue("pageTitle");
            ViewBag.PageSummary = CurrentPage.GetPropertyValue("pageSummary");
            ViewBag.Faqs = CurrentPage.GetPropertyValue("faqs");
            ViewBag.Downloads = CurrentPage.GetPropertyValue("downloads");
            ViewBag.RelatedLinks = CurrentPage.GetPropertyValue("relatedLinks").ToString();

            var model = new MakePaymentViewModel() { Subscriptions = new List<OrderSubscriptionModel>() };

            var result = _industryDataInterchange.GetFinancialHistory(SessionHelper.AccountId, orderId.Value);
            var order = result.Orders.FirstOrDefault(o => o.OrderID == orderId.Value);
            if (result.Errors != null || order == null)
            {
                //TODO: How do we handle errors?
            }
            
            else
            {
                ViewBag.CanPay = OrderIsPayable(order);// TODO: how do we handle cannot pay?
                model.OrderTotal = order.Amount;
                model.PaidToDate = order.PaymentReceived;
                model.OrderBalance = order.PaymentOutstanding;
                model.OrderId = order.OrderID;
                model.OrderNumber = order.OrderNumber;

                foreach (var subscription in order.OrderLines)
                {
                    model.Subscriptions.Add(new OrderSubscriptionModel()
                    {
                        Price = subscription.Price,
                        Type = subscription.SubscriptionName,
                        Washes = subscription.WashCredits,
                    });
                }
            }

            return PartialView("MakePayment", model);
        }

        [NotChildAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MakePayment(MakePaymentViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                var confirmationUrl = CurrentPage.Children.Any() ? CurrentPage.Children.First().Url : null;
                var host = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority;

                var accountId = SessionHelper.AccountId;
                if (!String.IsNullOrEmpty(confirmationUrl) && !String.IsNullOrEmpty(host) && accountId > 0)
                {
                    //Create Payment
                    string transactionId;
                    if (_creditCardPaymentService.CreatePayment(model.OrderId, model.OrderNumber, accountId, (int)(model.OrderBalance * 100), out transactionId) && !String.IsNullOrEmpty(transactionId))
                    {
                        //Redirect Access Seeker
                        var url = _paymentGatewayService.GetPaymentGatewayUrl(transactionId, model.OrderNumber, (int)(model.OrderBalance * 100), host + confirmationUrl);
                        if (!String.IsNullOrEmpty(url))
                            return new RedirectResult(url);
                    }
                }
            }
            TempData.Add("SubscriptionViewModel", model);
            return CurrentUmbracoPage();
        }

        [ChildActionOnly]
        public ActionResult MakePaymentConfirmation(string receipt)
        {
            var home = CurrentPage.AncestorOrSelf(1);
            IPublishedContent accountDashboardNode = Umbraco.Content(home.GetPropertyValue("accountDashboardNode"));
            IPublishedContent makePaymentNode = Umbraco.Content(home.GetPropertyValue("makePaymentNode"));
            ViewBag.AccountDashboardURL = accountDashboardNode.Url;
            ViewBag.MakePaymentURL = makePaymentNode.Url;

            var accountUserId = SessionHelper.AccountUserId;
            int? agentId = SessionHelper.AgentId;

            ViewBag.IsSuccess = false;
            //Access Seeker return
            if (!string.IsNullOrEmpty(receipt))
            {
                //VerifyReceipt
                string transactionId;
                Enums.PaymentStatusEnum paymentStatus;
                string response;
                string message;
                string creditCardReference;
                string receiptNumber;
                DateTime? settlementDate;
                long? transactionNo;
                int? transactionAmountInCents;
                string authorizeId;
                string transactionType;
                string cardType;
                _paymentGatewayService.VerifyReceipt(receipt, out transactionId, out paymentStatus, out response, out message, out creditCardReference, out receiptNumber, out settlementDate, out transactionNo, out transactionAmountInCents, out authorizeId, out transactionType, out cardType);

                //check if is has been processed
                var payment = _creditCardPaymentService.GetCreditCardPayment(transactionId);

                if (!payment.IsProcessed)
                {
                    int? orderId;
                    //Update Payment Status
                    _creditCardPaymentService.UpdateStatus(transactionId, receipt, response, message, receiptNumber,
                        settlementDate, paymentStatus, creditCardReference, transactionNo, transactionAmountInCents,
                        authorizeId, transactionType, cardType, out orderId);
                    ViewBag.OrderId = orderId;

                    if (paymentStatus == Enums.PaymentStatusEnum.SUCCESS)
                    {
                        if (orderId != null)
                        {
                            var args = new PayForOrderArguments()
                                       {
                                           AccountUserID = accountUserId,
                                           AgentUserID = agentId ?? 0,
                                           Amount = transactionAmountInCents/100 ?? 0,
                                           AuthorizeID = authorizeId,
                                           CardType = cardType,
                                           CreditCardReference = creditCardReference,
                                           ReceiptNo = receiptNumber,
                                           IsCSR = agentId != null,
                                           OrderID = orderId.Value,
                                           ResponseCode = response,
                                           TransactionID = transactionId,
                                           TransactionNo = transactionNo.Value,
                                           TransactionType = transactionType,
                                           ExternalMessage = message,
                                           ExpectedSettelmentDate = settlementDate,
                                       };

                            var result = _industryDataInterchange.PayForOrder(args);

                            if (result.IsSuccessful || (!result.IsSuccessful && result.Errors != null))
                                _creditCardPaymentService.SetTransactionAsProcessed(transactionId, result.IsSuccessful,
                                    result.Errors != null ? result.Errors.Message : null);
                        }
                        ViewBag.IsSuccess = true;
                        ViewBag.OrderId = orderId;
                    }
                    else if (paymentStatus == Enums.PaymentStatusEnum.DECLINED)
                    {
                        if (orderId != null)
                        {
                            var args = new PayForOrderArguments()
                                       {
                                           AccountUserID = accountUserId,
                                           AgentUserID = agentId ?? 0,
                                           Amount = transactionAmountInCents/100 ?? 0,
                                           AuthorizeID = authorizeId,
                                           CardType = cardType,
                                           CreditCardReference = creditCardReference,
                                           ReceiptNo = receiptNumber,
                                           IsCSR = agentId != null,
                                           OrderID = orderId.Value,
                                           ResponseCode = response,
                                           TransactionID = transactionId,
                                           TransactionNo = transactionNo.Value,
                                           TransactionType = transactionType,
                                           ExternalMessage = message,
                                           ExpectedSettelmentDate = settlementDate,
                                       };
                            var result = _industryDataInterchange.RecordPayForOrderFail(args);
                            if (result.IsSuccessful || (!result.IsSuccessful && result.Errors != null))
                                _creditCardPaymentService.SetTransactionAsProcessed(transactionId, result.IsSuccessful,
                                    result.Errors != null ? result.Errors.Message : null);
                        }
                    }
                }
                else
                {
                    ControllerContext.HttpContext.Response.Redirect(accountDashboardNode.Url);
                }
            }
            return PartialView("MakePaymentConfirmation");
        }

        #endregion

        #region Download Invoice

        public ActionResult GenerateInvoice(int orderId)
        {
            var accountId = SessionHelper.AccountId;
            if (accountId == 0 || SessionHelper.AccountUserId == 0)
            {
                return new RedirectResult("/");
            }

            var orderResult = _industryDataInterchange.GetFinancialHistory(accountId, orderId);
            if (orderResult.Errors != null)
            {
                //TODO: How do we handle errors?
            }
            else if (orderResult.Orders != null && orderResult.Orders.FirstOrDefault().OrderID == orderId)
            {
                var orderNum = orderResult.Orders.FirstOrDefault().OrderNumber;
                var result = _industryDataInterchange.GenerateInvoice(orderId);
                if (result != null && result.InvoiceStream != null)
                {
                    return File(result.InvoiceStream, System.Net.Mime.MediaTypeNames.Application.Octet, String.Format("DNCR_Order_{0}.pdf", orderNum));
                }
            }
            return null;
        }

        #endregion
    }
}
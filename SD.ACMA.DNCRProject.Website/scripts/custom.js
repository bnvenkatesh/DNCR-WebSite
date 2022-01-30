$(function () {
    var content = $('scrollDiv');
    content.on('touchstart', function(event) {
        this.allowUp = (this.scrollTop > 0);
        this.allowDown = (this.scrollTop < this.scrollHeight - this.clientHeight);
        this.slideBeginY = event.pageY;
    });

    content.on('touchmove', function(event) {
        var up = (event.pageY > this.slideBeginY);
        var down = (event.pageY < this.slideBeginY);
        this.slideBeginY = event.pageY;
        if ((up && this.allowUp) || (down && this.allowDown)) {
            event.stopPropagation();
        } else {
            event.preventDefault();
        }
    });

    if ($(window).width() < 768) {
        $(".navHolder").width($(window).width());
    } else {
        $(".navHolder").width("225px");
    }

    $(".dialog").dialog({
        autoOpen: false,
        resizable: false,
        width: 'auto',
        //width: 568,
        modal: true,
        open: function() {
            $('.ui-widget-overlay').bind('click', function() {
                jQuery('.dialog').dialog('close');
            })
        },
        show: {
            effect: "blind",
            duration: 500
        },
        create: function () {
            // Set maxWidth
            $(this).css("maxWidth", "568px");
        }
    });

    $("#openUnlockUser").click(function (event) {
        event.preventDefault();
        $("#unlockUser").dialog("open");
    });

    $(".closeError").click(function() { //Close error messages
        $(this).parent().remove();
    });

    function setHeightOfMenu() {
        /* R10-UPDATE - no longer required
        var browserHeight = $(window).innerHeight() - 111;
        var navHolderHeight = browserHeight - 113;
        $(".navHolder").css("min-height", navHolderHeight);
        */
    }

    function showMenuNavigation() {
        $("#menu").toggle();

        if ($('#menu').is(':visible')) {
            $(".content").addClass("contentPadding");
            $(".content").addClass("mobilePadding");
            var newUserInfoBarWidth = getPercentage(275, browserWidth);
        }
        else {
            $(".content").removeClass("contentPadding");
            $(".content").removeClass("mobilePadding");
            var newUserInfoBarWidth = getPercentage(50, browserWidth);
        }
        newUserInfoBarWidth = 100 - newUserInfoBarWidth;
        $(".userInfoBar").css("width", newUserInfoBarWidth + "%");
    }

    setHeightOfMenu();

    $(".imgHover").on({
        "mouseover": function () {
            this.src = $(this).attr("data-img-src") + "-hover.png";
        },
        "mouseout": function () {
            this.src = $(this).attr("data-img-src") + ".png";
        }
    });

    $("a").on({
        "focusin": function () {
            if ($(this).find(".imgHover").length > 0) {
                $(this).find(".imgHover").attr("src", $(this).find(".imgHover").attr("data-img-src") + "-hover.png");
            }
        },
        "focusout": function() {
            if ($(this).find(".imgHover").length > 0) {
                $(this).find(".imgHover").attr("src", $(this).find(".imgHover").attr("data-img-src") + ".png");
            }
        }
    });

    var slider = $('.bxslider').bxSlider({
        mode: 'fade',
        speed: 1000,
        auto: true,
        pager: true,
        controls: false,
        autoControlsCombine: true,
        autoControls: false,
        onSlideBefore: function (slideElement) {
                changeBackground(slideElement, 1000);
        }
    });

    var cssCheck;
    function detectCssOff() {
        if($("body").css('color') === "rgb(0, 0, 0)") {
            $("nav").remove();
            $(".cssOn").remove();
            $(".tablet, .searchButtonTablet, .showOnTablet").remove();
            $(".mobile, .consumerContactMobile, .mobileDesc, .mobileSearch, .searchButtonMobile, .showOnMobile").remove();
            if (slider != null) {
                slider.destroySlider();
            }
            clearInterval(cssCheck);
        }
        else if ($(".headerLogo:visible").width() < 300) {
            $("a.bx-pager-link, a.bx-stop, a.bx-start").addClass("noIndent");
        }
    }
    cssCheck = setInterval(detectCssOff, 1000);

    var browserWidth = $( window ).width() + 17;
			
	$("#tabIndustry").hide();
	$("#tabConsumers").show();
			
	if($(window).width() < 768){			
	$(".tabsHolder .tabButtons").width($( window ).width() - 30 + "px");
	}
	function getPercentage(initialWidth, fullWidth){			
		var percentValue = (initialWidth / fullWidth) * 100;				
		var roundedvalue = percentValue.toFixed(5);				
		return roundedvalue;
	}
			
	function getChildDivPercentage(childDiv){
			
		var percentValue = 100 - childDiv;				
		var roundedvalue = percentValue.toFixed(5);				
		return roundedvalue;
	}			
				
				
	var menuButton = getPercentage(50, browserWidth);	
			
	if($(window).width() > 1025){
		var userInfoBarWidth = getPercentage(275, browserWidth);		
		userInfoBarWidth = 100 - userInfoBarWidth;
	}else if($(window).width() < 768){
		var userInfoBarWidth = getPercentage(0, browserWidth);		
		userInfoBarWidth = 100 - userInfoBarWidth;
	}else{
		var userInfoBarWidth = getPercentage(55, browserWidth);		
		userInfoBarWidth = 100 - userInfoBarWidth;
				
	}
	$(".userInfoBar").css("width",userInfoBarWidth+"%");
			
	/*show/hide menu section*/
    $(".tabsContent .ul-icons img")
        .mouseout(function() {
            if ($(this).is('.desk')) {
                var currentAttr = $(this).attr("src");
                var newSrc = currentAttr.replace('-hover.png', '');
                newSrc = newSrc.replace('.png', '');
                $(this).attr('src', newSrc + ".png");
            }
        })
        .mouseover(function() {
            if ($(this).is('.desk')) {
                var currentAttr = $(this).attr("src");
                var newSrc = currentAttr.replace('.png', '');
                var newSrc = newSrc.replace('-hover', '');
                $(this).attr('src', newSrc + "-hover.png");

            }
            $(this).css("cursor", "pointer");
        });

    $(".tabsContent .ul-icons a").on({
	    "focusin": function() {
	        if ($(this).find("img.desk").length > 0) {
	            var currentAttr = $(this).find("img.desk").attr("src");
	            var newSrc = currentAttr.replace('.png', '');
	            var newSrc = newSrc.replace('-hover', '');
	            $(this).find("img.desk").attr('src', newSrc + "-hover.png");
	        }
	    },
	    "focusout": function () {
            if ($(this).find("img.desk").length > 0) {
                var currentAttr = $(this).find("img.desk").attr("src");
                var newSrc = currentAttr.replace('-hover.png', '');
                newSrc = newSrc.replace('.png', '');
                $(this).find("img.desk").attr('src', newSrc + ".png");
            }
        }
    });
	/* end show/hide menu section */	
			
			
			
			
    $(".mobileMenu").click(function () {
        /*R10 - unused code
        if ($('#searchBar').is(".searchNovOn")) {
            $(".searchHolder").removeClass('searchNovOn');
            $(".searchHolder").addClass('searchNovOff');
            $("#header.iOSSafariFixed").removeClass("iOSSafariFixed");
            $("#menu.iOSSafariFixedMenus, #menuButton.iOSSafariFixedMenus").removeClass("iOSSafariFixedMenus");
        }
        */
		$( ".content" ).css("padding-left:","0px");	
		$(".acma-logo-holder").toggleClass("acma-logo-holder-dark");
		$(".mobileMenu").toggleClass("mobileMenu-dark");
		$(".searchHolder ").toggleClass("hideOnMobile");
		$("#menu").slideToggle();
		if ($(".acma-logo-holder").hasClass('acma-logo-holder-dark') /*&& navigator.userAgent.indexOf("Version/1.0") >= 0*/) {
			$("#sectionContent").css("height", $(window).height());
            $("#sectionContent").css("overflow", "hidden");
			$(window).scrollTop(0);
		} else {
			$("#sectionContent").css("height", "auto");
            $("#sectionContent").css("overflow", "visible");
        }
	});

    /* R10-UPDATE - Search button no longer expands and closes search box
	$( ".searchButtonMobile, .searchButtonTablet" ).click(function() {					
				
			if ($('#searchBar').is(".searchNovOn")) {
				$(".searchHolder").removeClass('searchNovOn');  
				$(".searchHolder").addClass('searchNovOff');  
					
			} else {
			$(".searchHolder").removeClass('searchNovOff');  
				$(".searchHolder").addClass('searchNovOn');  	
			}
				  
			if($(window).width() < 768){						
				$(".searchNovOn").width($( window ).width() - 58 + "px");
			}else{	
				$(".searchNovOn").width($( window ).width() - 455 + "px");
			}
	});
    */

	$( "#menuButton .menuIcon" ).click(function() {
		showMenuNavigation();
	});
	$("#menuButton .menuIcon").keydown(function (event) {
	    if (event.keyCode == 13) {
			showMenuNavigation();
		}
	});
			
	/* table */
			
	$("table").on("click", "tr td:nth-child(1)", function() {
		if($(window).width() < 1025){
			$("table tr.active").removeClass("active");
			$(this).parent().toggleClass("active");
		}
		});		
	/* table */
			
	/* Banner tabs */
		$(".tabButtons > div").click(function() {
				
			var tabSelected = $(this).attr("id");
			$(this).addClass( "active" );
			$(this).siblings().removeClass( "active" );
					
					
			if(tabSelected == "forConsumers"){
				$("#tabIndustry").hide();
				$("#tabConsumers").show();
			}else{
				$("#tabIndustry").show();
				$("#tabConsumers").hide();
			}
		});
				
				
	/* End banner tabs*/
			
	$(document).on('keydown', '*', function() {
		$("#faqAccordion .ui-accordion-header, #navAccordion .ui-accordion-header, .globalAccordion .ui-accordion-header").attr("tabindex",0);
	});
	var lastFocusedID;
	$(document).on('focus', '*', function() {
				
		$("#faqAccordion .ui-accordion-header, #navAccordion .ui-accordion-header, .globalAccordion .ui-accordion-header").attr("tabindex",0);
				
				
		/*lastFocusedID = $(this).attr("id");
		if(lastFocusedID == "forConsumers"){
			$(this).addClass( "active" );
			$(this).siblings().removeClass( "active" );
					
			$("#tabIndustry").hide();
			$("#tabConsumers").show();
		}else if(lastFocusedID == "forIndustry"){
			$(this).addClass( "active" );
			$(this).siblings().removeClass( "active" );
					
			$("#tabIndustry").show();
			$("#tabConsumers").hide();
					
		}*/

	});

    /* R10-UPDATE */
	function showSearchInMenu(show) {
	    const barSearch = $('#searchBar input[type=search]')[0];
	    const navSearch = $('#navSearchHolder input[type=search]')[0];

	    if (show) {
	        barSearch.id = "Keywords_disabled";
	        barSearch.name = "Keywords_disabled";
	        navSearch.id = "Keywords";
	        navSearch.name = "Keywords";
	    } else {
	        barSearch.id = "Keywords";
	        barSearch.name = "Keywords";
	        navSearch.id = "Keywords_disabled";
	        navSearch.name = "Keywords_disabled";
	    }
	}

    // Set initial search input settings
	showSearchInMenu($(window).width() < 768);

	/* Social adjusment  */
	
	var $window = $(window);
	var lastWidth = $window.width();

	$window.on('resize', function () {
	    if ($(window).width() != lastWidth) {

	        setHeightOfMenu();

	        if ($(window).width() < 768) {
	            $(".navHolder").width($(window).width());
	            $(".tabsHolder .tabButtons").width($(window).width() - 30 + "px");

	            $(".searchNovOn").width($(window).width() - 58 + "px");
	            showSearchInMenu(true);
	        } else {
	            $(".navHolder").width("225px");
	            $(".tabsHolder .tabButtons").css("width", "");
	            $(".searchNovOn").width($(window).width() - 455 + "px");
	            showSearchInMenu(false);
	        }


	        if ($(window).width() > 1025) {
	            var userInfoBarWidth = getPercentage(275, browserWidth);
	            userInfoBarWidth = 100 - userInfoBarWidth;
	        } else if ($(window).width() < 768) {
	            var userInfoBarWidth = getPercentage(0, browserWidth);
	            userInfoBarWidth = 100 - userInfoBarWidth;
	        } else {
	            var userInfoBarWidth = getPercentage(55, browserWidth);
	            userInfoBarWidth = 100 - userInfoBarWidth;

	        }
	        $(".userInfoBar").css("width", userInfoBarWidth + "%");

	        if ($('#searchBar').is(".searchNovOn")) {
	            $(".searchHolder").removeClass('searchNovOn');
	            $(".searchHolder").addClass('searchNovOff');

	        }

	        lastWidth = $window.width();
	    }
	});
	
	/* End Social adjusment  */	
	/* Accordion */			
	var icons = {
	  header: "ui-icon-default",
	  activeHeader: "ui-icon-active"			  
	};
	$( "#navAccordion" ).accordion({
	  icons: icons,
	  heightStyle: "content",
	  active: activeNavAccordionIndex,	
	  collapsible: true,
	  beforeActivate: function(event, ui) {
			var accordionIndex = $(this).find(".navItemLabel").index(ui.newHeader[0]);			
			// The accordion believes a panel is being opened
			if (ui.newHeader[0]) {
				var currHeader  = ui.newHeader;
				var currContent = currHeader.next('.ui-accordion-content');
			 // The accordion believes a panel is being closed
			} else {
				var currHeader  = ui.oldHeader;
				var currContent = currHeader.next('.ui-accordion-content');
			}
			var isPanelSelected = currHeader.attr('aria-selected') == 'true';
			
			$("#navAccordion li").siblings().removeClass('inactiveItem');	
			if(accordionIndex != activeNavAccordionIndex ){				
				currHeader.parent('.navItem').toggleClass('defaultItem',isPanelSelected).toggleClass('inactiveItem',!isPanelSelected);
			}		
			return true; 
		}		
	});

	$("#faqAccordion .ui-accordion-header, #navAccordion .ui-accordion-header, .globalAccordion .ui-accordion-header").attr("tabindex",0);
 

	var icons = {
	  header: "ui-icon-default",
	  activeHeader: "ui-icon-active"			  
	};
	$( "#faqAccordion" ).accordion({
		icons: icons,
		heightStyle: "content",
		active:true,
		collapsible:true
	});

	var icons = {
	  header: "ui-icon-default-dashboard",
	  activeHeader: "ui-icon-active-dashboard"			  
	};
	$( "#dashBoardAccordion" ).accordion({
	  icons: icons,
	  heightStyle: "content",
	  active: false,
	  collapsible: true,
	  beforeActivate: function (event, ui) {
	      // The accordion believes a panel is being opened
	      if (ui.newHeader[0]) {
	          var currHeader = ui.newHeader;
	          var currContent = currHeader.next('.ui-accordion-content');
	          // The accordion believes a panel is being closed
	      } else {
	          var currHeader = ui.oldHeader;
	          var currContent = currHeader.next('.ui-accordion-content');
	      }
	      // Since we've changed the default behavior, this detects the actual status
	      var isPanelSelected = currHeader.attr('aria-selected') == 'true';

	      // Toggle the panel's header
	      currHeader.toggleClass('ui-corner-all', isPanelSelected).toggleClass('ui-accordion-header-active ui-state-active ui-corner-top', !isPanelSelected).attr('aria-selected', ((!isPanelSelected).toString()));

	      // Toggle the panel's icon
	      currHeader.children('.ui-icon').toggleClass('ui-icon-triangle-1-e', isPanelSelected).toggleClass('ui-icon-triangle-1-s', !isPanelSelected);

	      // Toggle the panel's content
	      currContent.toggleClass('accordion-content-active', !isPanelSelected)
	      if (isPanelSelected) { currContent.slideUp(); } else { currContent.slideDown(); }

	      return false; // Cancels the default action
	  }
	});
	
	var iOS = ( navigator.userAgent.match(/(iPad|iPhone|iPod)/g) ? true : false );
/* R10-UPDATE	
	if(iOS){
		$( "input, textarea" ).on('focus', function(){	
			$(".content").removeClass("contentPadding");
			$(".content").removeClass("mobilePadding");
			
			$("#menu").hide();

			$("#header").addClass("iOSSafariFixed");
			
			$("#menu, #menuButton").addClass("iOSSafariFixedMenus");
	
		});
		
		$( "input, textarea" ).on('blur', function(){	
		    $("#header.iOSSafariFixed").removeClass("iOSSafariFixed");
		    $("#menu.iOSSafariFixedMenus, #menuButton.iOSSafariFixedMenus").removeClass("iOSSafariFixedMenus");
		});
		
		
	}
*/
    //prevent outline on mouse click

    $(document).on("mousedown", "*", function(e) {
        if (($(this).is(":focus") || $(this).is(e.target)) && $(this).css("outline-style") == "none") {
            $(this).css("outline", "none").on("blur", function() {
                $(this).off("blur").css("outline", "");
            });
        }
        if ($(this).attr("for") != null) {
            var that = $("#" + $(this).attr("for"));
            if (that.length > 0) {
                that.css("outline", "none").on("blur", function() {
                    that.off("blur").css("outline", "");
                });
            }
        }
    });
	
});

		
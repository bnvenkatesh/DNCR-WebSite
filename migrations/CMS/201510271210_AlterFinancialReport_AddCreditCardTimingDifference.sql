alter table FinancialReport add  CreditCardTimingDifference money 
go

update FinancialReport set CreditCardTimingDifference = 0
go

alter table FinancialReport alter column CreditCardTimingDifference money not null
go

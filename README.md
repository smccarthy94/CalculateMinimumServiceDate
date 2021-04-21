# CalculateMinimumServiceDate
## Find a minimum booking date, while taking into account a given notice period and public holidays (singular and consecutive)/weekends

To run unit tests, navigate to `ServiceDate.Services` and execute `dotnet test`.
Tests verify the following cases: 
1. Monday, before sameday cut off
1. Monday, after sameday cut off
1. Friday, before sameday cut off
1. Friday, after sameday cut off
1. Wednesday before Easter, before sameday cut off
1. Wednesday before Easter, after sameday cut off

API to retrieve a minimum date from "Now":
`GET:http://localhost:60151/booking/minimum-service-date/123`

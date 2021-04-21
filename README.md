# CalculateMinimumServiceDate

To run unit tests, navigate to `ServiceDate.Services` and execute `dotnet test`.
Tests verify the following cases: 
1. Monday, before sameday cut off
1. Monday, after sameday cut off
1. Friday, before sameday cut off
1. Friday, after sameday cut off
1. Wednesday before Easter, before sameday cut off
1. Wednesday before Easter, after sameday cut off

Service is written in such a way that it could be consumed by a client/API at a later stage via depedency injection.

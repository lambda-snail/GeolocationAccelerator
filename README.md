
[![Build Status](https://dev.azure.com/blombergniclas/GeoLocation.Accelerator/_apis/build/status/lambda-snail.GeolocationAccelerator?branchName=main)](https://dev.azure.com/blombergniclas/GeoLocation.Accelerator/_build/latest?definitionId=3&branchName=main)

# GeoLocation Accelerator

This is a simple project to enable location-based queries in Dynamics 365 and powerapps.

There may be other solutions out there that do the same thing, but this one will be released as open source - feel free to extend it to suit your own purposes.

The tables are very bare-bones, but can of course be extended with more bussiness logic, such as columns for location type and so on.

## Roadmap

### Version 1.0

- A few simple Dynamics tables that define locations and regions using latitude and longitude coordinates.
- A Cosmos Db that enables geospatial queries.
- A layer of Azure functions that sits between Dynamics and Cosmos Db.
- Also there will be either a plugin or javascript functions (or both) that allows us to interact with the layer in Azure.

### Version 1.1

- Graphic visualization of locations and regions on a map.

## Notes

As this project just started, som things are missing, such as a dynamics solution. The database is also missing from the arm templates. These things will be added in due time.

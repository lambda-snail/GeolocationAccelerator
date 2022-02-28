
[![Build Status](https://dev.azure.com/blombergniclas/GeoLocation.Accelerator/_apis/build/status/lambda-snail.GeolocationAccelerator?branchName=main)](https://dev.azure.com/blombergniclas/GeoLocation.Accelerator/_build/latest?definitionId=3&branchName=main)

# GeoLocation Accelerator

This is a simple project to enable location-based queries in Dynamics 365 and powerapps.

There may be other solutions out there that do the same thing, but this one will be released as open source - feel free to extend it to suit your own purposes.

The tables are very bare-bones, but can of course be extended with more bussiness logic, such as columns for location type and so on.

## Roadmap

A progress report can be found in the [projects section](https://github.com/users/lambda-snail/projects/1) of this repo. The immediate goals are as follows.

### Version 1.0

- A few simple Dynamics tables that define locations and regions using latitude and longitude coordinates.
- An API that enables geospatial queries - powered by a cosmos db and some simple Azure functions.
- A simple javascript (typescript) library that provides a service to interact with the layer in Azure.

### Version 1.1

- Graphic visualization of locations and regions on a map.

### Version 1.2

- Refactor to facilitate configuration, in particular which fields are synced to the database.

# Deployment and configuration

The API is not yet in a useable state. For version 1.0, configuration will be done by altering the source code. For version 1.2+ some kind of mechanism to facilitate configuration will be implemented. Storing more data from the CRM in cosmos will allow more interesting queries, so making that process simpler is certainly on my roadmap!

## Notes

As this project just started, som things are missing, such as a dynamics solution. The database is also missing from the arm templates. These things will be added in due time.

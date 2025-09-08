## Qanat.R

R Interface to the REST API for the Groundwater Accounting Platform (codename "Qanat") based on [`httr2` package](https://httr2.r-lib.org/).

### Requirements

The `Qanat.R` package requires R version 4.1 or newer. The package is only available on GitHub, but can be installed with the [`remotes` package](https://remotes.r-lib.org/). The [`sf` package](https://r-spatial.github.io/sf/) is suggested if you are working with the spatial data provided by the API. 

### Installation

```
remotes::install_github("thinkelman-ESA/qanat-community@r-package", subdir = "Qanat.R")
```

### Authentication

If you haven't already, create an API key via the [Groundwater Accounting Platform](https://www.groundwateraccounting.org/getting-started-with-the-api). After loading the package with `libarary(Qanat.R)`, run `set_user_key()` to open a dialog box where you can paste the key that you copied from the Groundwater Accounting Platform. After setting your key, you can confirm that it worked by running `get_user_key()`. 

To avoid setting the key every time, add `X-QANAT-KEY=put_your_unique_key_here` to the .Renviron file. If you are new to editing the .Renviron file, see [`usethis::edit_r_environ()`](https://usethis.r-lib.org/reference/edit.html). 

### Demo geography

Demo geography is provided that allows anyone to explore functionality and features. To request access to the Demo geography, please contact info@groundwateraccounting.org. All examples provided by the `Qanat.R` package use the Demo geography.

### Wrapping the API

The `Qanat.R` package wraps the API endpoints into R functions to simplify working with the API. All of the functions perform `GET` requests that return either JSON or GeoJSON objects. By default, all `Qanat.R` functions return data frames (spatial data frames in the case of GeoJSON objects). Setting `simplify = FALSE` returns a list representation of the JSON object rather than a data frame. Setting `sf = FALSE` returns a list representation of the GeoJSON object; a simplified version of the list is returned when `sf = FALSE` and `simplify = TRUE`.

```
> library(Qanat.R)

> geographies(simplify = TRUE)

  GeographyID GeographyName GeographyDisplayName
1           5          Demo       Demo Geography

> geographies(simplify = FALSE)

[[1]]
[[1]]$GeographyID
[1] 5

[[1]]$GeographyName
[1] "Demo"

[[1]]$GeographyDisplayName
[1] "Demo Geography"
```

### HTTP Errors

The package is a thin wrapper around the API endpoints and returns [HTTP error codes](https://en.wikipedia.org/wiki/List_of_HTTP_status_codes) rather than trying to catch potentially malformed requests. One common error involves an incorrect user key.

```
> geographies(user_key = "not-a-valid-key")

Error in `httr2::req_perform()` at Qanat.R/R/private.R:13:3:
! HTTP 401 Unauthorized.
```

Another example involves resources that are not found. For example, the demo geography only includes one geography ID. Any integer other than 5 will not be found.

```
> parcels(geography_id = 100)

Error in `httr2::req_perform()` at Qanat.R/R/private.R:13:3:
! HTTP 404 Not Found.
```

Because the parcels endpoint expects an integer for geography ID, you will get a bad request code if you provide anything other than an integer. Note, though, that you will get a valid result if you pass the geography ID as a string representation of the integer.

```
> parcels(geography_id = "not-a-geo-id")

Error in `httr2::req_perform()` at Qanat.R/R/private.R:13:3:
! HTTP 400 Bad Request.

> head(parcels(geography_id = "5"))

  ParcelID ParcelNumber ParcelArea                      OwnerName                                   OwnerAddress WaterAccountID                           Zones GeographyID
1    52473   555-042-93   626.9132              Crop Circle Farms        1234 Olive Drive, Bakersfield, CA 93308             13 13, Zone 3, 4, Management Zones           5
2    52474   555-043-82   237.1462              Berry Nutty Farms  5555 Stockdale Highway, Bakersfield, CA 93309              6 14, Zone 4, 4, Management Zones           5
3    52475   555-044-68     8.6882            Melon Madness Farms        7890 Ming Avenue, Bakersfield, CA 93309             30 12, Zone 2, 4, Management Zones           5
4    52476   555-045-44    56.4169 The Jolly Green Giant's Garden     2345 Chester Avenue, Bakersfield, CA 93301             50 13, Zone 3, 4, Management Zones           5
5    52477   555-046-93   100.3180               Sweet Pea's Farm           6789 H Street, Bakersfield, CA 93304             49 12, Zone 2, 4, Management Zones           5
6    52478   555-047-95   459.8577           Lavender Fields Farm      1515 Bluebird Lane, Bakersfield, CA 93305             29 14, Zone 4, 4, Management Zones           5
```

### Examples

The `Qanat.R` package includes examples as R Markdown files to demonstrate how to use the package. The examples require the following packages: dplyr, lubridate, sf, mapview, ggplot2, plotly, reactable. The examples can be found on [GitHub](https://github.com/thinkelman-ESA/qanat-community/tree/r-package/Qanat.R/inst/examples), but are also available locally after installing the package. Run the following code in the R console to find where the examples are stored locally.

```
system.file("examples", package = "Qanat.R")
```

### Manual pages

Browse the `Qanat.R` manual pages.

```
help(package = "Qanat.R")
```

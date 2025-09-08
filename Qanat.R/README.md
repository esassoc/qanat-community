## Qanat.R

R Interface to the REST API for the Groundwater Accounting Platform (codename "Qanat") based on [`httr2` package](https://httr2.r-lib.org/).

Install from GitHub with the [`remotes` package](https://remotes.r-lib.org/).

```
remotes::install_github("thinkelman-ESA/qanat-community@r-package", subdir = "Qanat.R")
```

Browse the `Qanat.R` manual pages.

```
help(package = "Qanat.R")
```

The `Qanat.R` package includes examples as R Markdown files to demonstrate how to use the package. The examples require the following packages: dplyr, lubridate, sf, mapview, ggplot2, plotly, reactable. The examples can be found on [GitHub](https://github.com/thinkelman-ESA/qanat-community/tree/r-package/Qanat.R/inst/examples), but are also available locally after installing the package. Run the following code in the R console to find where the examples are stored locally.

```
system.file("examples", package = "Qanat.R")
```



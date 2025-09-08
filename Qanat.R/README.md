## Qanat.R

R Interface to the REST API for the Groundwater Accounting Platform (codename "Qanat") based on [`httr2` package](https://httr2.r-lib.org/).

Install from GitHub with the [`remotes` package](https://remotes.r-lib.org/).

```
remotes::install_github("thinkelman-ESA/qanat-community@r-package", subdir = "Qanat.R")
```

The `Qanat.R` package includes vignettes to demonstrate how to use the package. The vignettes require additional packages: dplyr, lubridate, sf, mapview, ggplot2, plotly, reactable. If you have already installed those packages, you build the vignettes during the installation of `Qanat.R`.

```
remotes::install_github("thinkelman-ESA/qanat-community@r-package", subdir = "Qanat.R", 
                        build_vignettes = TRUE)
```

After installing with vignettes built, you can browse the vignettes.

```
browseVignettes(package = "Qanat.R")
```

Similarly, you browse the `Qanat.R` manual pages.

```
help(package = "Qanat.R")
```


#' Usage locations list
#'
#' List all usage locations for a specified geography and reporting period
#'
#' @md
#' @param geography_id  Geography ID
#' @param year          Four-digit year
#' @param simplify      Coerce to vector, data frame, or matrix
#' @param user_key      Qanat user key
#' @export

usage_locations <- function(geography_id, year, simplify = TRUE, user_key = get_user_key()) {
  glue::glue("{api_url()}/geographies/{geography_id}/years/{year}/usage-locations") |>
    get_qanat(simplify, user_key)
}

#' Usage locations feature collection
#'
#' List all usage locations as a feature collection for a specified geography and reporting period
#'
#' @md
#' @param geography_id  Geography ID
#' @param year          Four-digit year
#' @param sf            Return object of class sf (requires sf package)
#' @param simplify      Coerce to vector, data frame, or matrix. Ignored when sf = TRUE.
#' @param user_key      Qanat user key
#' @export

usage_locations_features <- function(geography_id, year, sf = TRUE, simplify = TRUE,
                                     user_key = get_user_key()) {
  glue::glue("{api_url()}/geographies/{geography_id}/years/{year}/\\
             usage-locations/feature-collection") |>
    get_qanat(simplify, user_key, sf)
}

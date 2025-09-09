
#' Wells list
#'
#' List all wells for a specified geography
#'
#' @md
#' @param geography_id  Geography ID
#' @param simplify      Coerce to vector, data frame, or matrix
#' @param user_key      Qanat user key
#' @export

wells <- function(geography_id, simplify = TRUE, user_key = get_user_key()) {
  get_by_geo(geography_id, simplify, user_key, "wells")
}

#' Wells feature collection
#'
#' List wells as a feature collection for a specified geography
#'
#' @md
#' @param geography_id  Geography ID
#' @param sf            Return object of class sf (requires sf package)
#' @param simplify      Coerce to vector, data frame, or matrix. Ignored when sf = TRUE.
#' @param user_key      Qanat user key
#' @export

wells_features <- function(geography_id, sf = TRUE, simplify = TRUE, user_key = get_user_key()) {
  glue::glue("{api_url()}/geographies/{geography_id}/wells/feature-collection") |>
    get_qanat(simplify, user_key, sf)
}

#' Meter readings list
#'
#' Search all meter readings for a specified geography and well
#'
#' @md
#' @param geography_id  Geography ID
#' @param well_id       Well ID
#' @param simplify      Coerce to vector, data frame, or matrix
#' @param user_key      Qanat user key
#' @export

meter_readings <- function(geography_id, well_id, simplify = TRUE, user_key = get_user_key()) {
  glue::glue("{api_url()}/geographies/{geography_id}/wells/{well_id}/meter-readings") |>
    get_qanat(simplify, user_key)
}

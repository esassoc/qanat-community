
#' Parcels list
#'
#' List all parcel numbers for a specified geography
#'
#' @md
#' @param geography_id  Geography ID
#' @param simplify      Coerce to vector, data frame, or matrix
#' @param user_key      Qanat user key
#' @export

parcels <- function(geography_id, simplify = TRUE, user_key = get_user_key()) {
  get_by_geo(geography_id, simplify, user_key, "parcels")
}

#' Parcels feature collection
#'
#' List parcels as a feature collection for a specified geography
#'
#' @md
#' @param geography_id  Geography ID
#' @param sf            Return object of class sf (requires sf package)
#' @param simplify      Coerce to vector, data frame, or matrix. Ignored when sf = TRUE.
#' @param user_key      Qanat user key
#' @export

parcels_features <- function(geography_id, sf = TRUE, simplify = TRUE, user_key = get_user_key()) {
  req = glue::glue("{api_url()}/geographies/{geography_id}/parcels/feature-collection") |>
    get_qanat(simplify, user_key, sf)
}

#' Parcels search
#'
#' Search all available parcel numbers for a specified geography and (partial) parcel number
#'
#' @md
#' @param geography_id  Geography ID
#' @param parcel_number Parcel number (minimum first 3 characters to search)
#' @param simplify      Coerce to vector, data frame, or matrix
#' @param user_key      Qanat user key
#' @export

parcels_search <- function(geography_id, parcel_number, simplify = TRUE,
                           user_key = get_user_key()) {
  glue::glue("{api_url()}/geographies/{geography_id}/parcels/{parcel_number}") |>
    get_qanat(simplify, user_key)
}

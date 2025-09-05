
#' Parcels list
#'
#' List all parcel numbers for a specified geography and, optionally, water account
#'
#' @md
#' @param geography_id       Geography ID
#' @param water_account_id   Water account ID (optional)
#' @param simplify           Coerce to vector, data frame, or matrix
#' @param user_key           Qanat user key
#' @export

parcels <- function(geography_id, water_account_id = NULL, simplify = TRUE, user_key = get_user_key()) {
  if (is.null(water_account_id)){
    get_by_geo(geography_id, simplify, user_key, "parcels")
  } else {
    glue::glue("{api_url()}/geographies/{geography_id}/water-accounts/{water_account_id}/parcels") |>
      get_qanat(simplify, user_key)
  }
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
  glue::glue("{api_url()}/geographies/{geography_id}/parcels/feature-collection") |>
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


#' Water measurement types list
#'
#' List all water measurement types for a specified geography
#'
#' @md
#' @param geography_id  Geography ID
#' @param simplify      Coerce to vector, data frame, or matrix
#' @param user_key      Qanat user key
#' @export

water_measurement_types <- function(geography_id, simplify = TRUE, user_key = get_user_key()) {
  get_by_geo(geography_id, simplify, user_key, "water-measurement-types")
}

#' Water measurements list
#'
#' List all water measurements for a specified geography and parcel or
#' specified geography, water measurement type, and year.
#' If parcel_id is not specified, water measurement type and year must be specified.
#'
#' @md
#' @param geography_id                  Geography ID
#' @param parcel_id                     Parcel ID
#' @param water_measurement_type_id     Water measurement type ID
#' @param year                          Four-digit year
#' @param simplify                      Coerce to vector, data frame, or matrix
#' @param user_key                      Qanat user key
#' @export
#' @examples
#' by_parcel <- water_measurements(geography_id = 5, parcel_id = 52475)
#' by_type_year <- water_measurements(geography_id = 5, water_measurement_type_id = 5, year = 2016)

water_measurements <- function(geography_id, parcel_id = NULL, water_measurement_type_id = NULL,
                               year = NULL, simplify = TRUE, user_key = get_user_key()) {

  if (is.null(parcel_id) && is.null(water_measurement_type_id) && is.null(year)){
    stop("Need to specify parcel_id or water_measurement_type_id and year.")
  }

  if (!is.null(parcel_id)){
    message("Listing measurements by geography and parcel.")
    url = glue::glue("{api_url()}/geographies/{geography_id}/parcels/{parcel_id}/water-measurements")
  } else {
    if (is.null(water_measurement_type_id) || is.null(year)){
      stop("Need to specify water_measurement_type_id and year")
    }
    message("Listing measurements by geography, water measurement type, and year.")
    url = glue::glue("{api_url()}/geographies/{geography_id}/years/{year}/\\
                     water-measurement-types/{water_measurement_type_id}/water-measurements")
  }

  get_qanat(url, simplify, user_key)
}

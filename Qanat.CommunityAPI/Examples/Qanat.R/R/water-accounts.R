
#' Water accounts list
#'
#' List all water accounts for a specified geography
#'
#' @md
#' @param geography_id  Geography ID
#' @param simplify      Coerce to vector, data frame, or matrix
#' @param user_key      Qanat user key
#' @export

water_accounts <- function(geography_id, simplify = TRUE, user_key = get_user_key()) {
  get_by_geo(geography_id, simplify, user_key, "water-accounts")
}

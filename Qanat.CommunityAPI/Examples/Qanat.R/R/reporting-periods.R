
#' Reporting periods list
#'
#' List all reporting periods for a specified geography
#'
#' @md
#' @param geography_id  Geography ID
#' @param simplify      Coerce to vector, data frame, or matrix
#' @param user_key      Qanat user key
#' @export

reporting_periods <- function(geography_id, simplify = TRUE, user_key = get_user_key()) {
  get_by_geo(geography_id, simplify, user_key, "reporting-periods")
}


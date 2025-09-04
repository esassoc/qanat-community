
#' Geographies list
#'
#' List of all available geographies
#'
#' @md
#' @param simplify      Coerce to vector, data frame, or matrix
#' @param user_key      Qanat user key
#' @export

geographies <- function(simplify = TRUE, user_key = get_user_key()) {
  glue::glue("{api_url()}/geographies") |>
    get_qanat(simplify, user_key)
}

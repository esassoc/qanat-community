
#' Geographies list
#'
#' List of all available geographies
#'
#' @md
#' @param simplify      Coerce to vector, data frame, or matrix
#' @param user_key      Qanat user key
#' @param use_qa        Boolean flag to use QA version of Qanat
#' @export

geographies <- function(simplify = TRUE, user_key = get_user_key(), use_qa = TRUE) {
  glue::glue("{api_url(use_qa)}/geographies") |>
    get_qanat(user_key) |>
    httr2::resp_body_json(simplifyVector = simplify)
}



# Private Functions -------------------------------------------------------

api_url <- function(use_qa = TRUE){
  # returns base url
  qa = if (use_qa) "-qa" else ""
  glue::glue("https://api{qa}.groundwateraccounting.org")
}

get_qanat <- function(url, simplify, user_key, sf = FALSE,
                      content_type = "application/octet-stream"){
  req = httr2::request(url) |>
    httr2::req_headers("X-API-KEY" = user_key, "Content-Type" = content_type) |>
    httr2::req_user_agent("Qanat.R") |>
    httr2::req_perform()

  if (sf) {
    sf::st_read(httr2::resp_body_string(req), quiet = TRUE)
  } else {
    httr2::resp_body_json(req, simplifyVector = simplify)
  }
}

get_by_geo <- function(geography_id, simplify, user_key, element) {
  glue::glue("{api_url()}/geographies/{geography_id}/{element}") |>
    get_qanat(simplify, user_key)
}


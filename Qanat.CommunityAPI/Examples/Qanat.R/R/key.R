
#' Set or get Qanat API key
#'
#' Set or get Qanat API key as environmental variable
#'
#' @param user_key     Qanat user key
#'
#' @export
set_user_key <- function(user_key = NULL) {
  if (is.null(user_key)) {
    user_key <- askpass::askpass("Please enter your Qanat user key")
  }
  Sys.setenv("X-QANAT-KEY" = user_key)
}

#' @rdname set_user_key
#' @export
get_user_key <- function() {
  user_key <- Sys.getenv("X-QANAT-KEY")
  if (!identical(user_key, "")) {
    return(user_key)
  } else {
    stop("No Qanat user key found, please supply with `user_key` argument or with
         X-QANAT-KEY environmental variable in .Renviron.")
  }
}

package com.digitall.eid.models.common

import com.digitall.eid.domain.models.base.ErrorType

data class PagingError(val title: StringSource,
                       val displayMessage: StringSource,
                       val originalException: Throwable?,
                       val errorType: ErrorType?): Exception(originalException)

/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.models.log

sealed class LogData {

    data class DebugMessage(
        val tag: String,
        val time: Long,
        val message: String,
    ) : LogData()

    data class ErrorMessage(
        val tag: String,
        val time: Long,
        val message: String,
    ) : LogData()

    data class ExceptionMessage(
        val tag: String,
        val time: Long,
        val exception: Throwable,
    ) : LogData()

    data class ErrorMessageWithException(
        val tag: String,
        val time: Long,
        val message: String,
        val exception: Throwable,
    ) : LogData()

    data class ErrorMessageWithData(
        val tag: String,
        val time: Long,
        val message: String?,
    ) : LogData()

    data class NetworkMessage(
        val time: Long,
        val message: String,
    ) : LogData()

}
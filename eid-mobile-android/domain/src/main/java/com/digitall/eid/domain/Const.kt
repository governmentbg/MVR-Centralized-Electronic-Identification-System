/**
 * these constants are collected here so that you can view any constant from this class at the same time
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain

import com.digitall.eid.domain.models.assets.localization.LocalizationsModel
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.domain.models.common.ApplicationEnvironment
import com.digitall.eid.domain.models.common.ApplicationLanguage
import com.digitall.eid.domain.models.common.MockResponse
import com.digitall.eid.domain.models.devices.DeviceModel
import java.util.TimeZone
import java.util.regex.Pattern

const val DEBUG_ACCOUNT_EMAIL = ""
const val DEBUG_ACCOUNT_PASSWORD = ""

val DEBUG_APPLICATION_CREATE_FIRST_LATIN_NAME: String? = null
val DEBUG_APPLICATION_CREATE_SECOND_LATIN_NAME: String? = null
val DEBUG_APPLICATION_CREATE_LAST_LATIN_NAME: String? = null
val DEBUG_APPLICATION_CREATE_BORN_DATE: Long? = null
val DEBUG_APPLICATION_CREATE_EGN: String? = null
val DEBUG_APPLICATION_CREATE_DOCUMENT_NUMBER: String? = null
val DEBUG_APPLICATION_CREATE_DOCUMENT_CREATED: Long? = null
val DEBUG_APPLICATION_CREATE_DOCUMENT_VALID: Long? = null


const val DEBUG_MOCK_INTERCEPTOR_ENABLED = false
const val DEBUG_LOGOUT_FROM_PREFERENCES = false
const val DEBUG_PRINT_PREFERENCES_INFO = true
const val DELAY_125 = 125L
const val DELAY_250 = 250L
const val DELAY_500 = 500L
const val DELAY_1000 = 1000L
const val DELAY_1500 = 1500L
const val DELAY_2500 = 2500L
const val SCAN_CARD_BOTTOM_SHEET_CONTENT_KEY = "SCAN_CARD_BOTTOM_SHEET_CONTENT_KEY"
const val INFORMATION_BOTTOM_SHEET_CONTENT_KEY = "INFORMATION_BOTTOM_SHEET_CONTENT_KEY"
const val EID_MOBILE_CERTIFICATE = "EID_MOBILE_CERTIFICATE"
const val EID_MOBILE_CERTIFICATE_KEYS = "EID_MOBILE_CERTIFICATE_KEYS"
const val CERTIFICATE_AUTHORITY = "MVR DEV CA"

val EMAIL_ADDRESS_PATTERN: Pattern = Pattern.compile(
    "[A-Z0-9a-z._%+\\-]+@[A-Za-z0-9.\\-]+\\.[A-Za-z]{2,64}"
)

val BG_PHONE_PATTERN: Pattern = Pattern.compile(
    "^(\\+359)+(8[789])+\\d{7}"
)

val PASSWORD_PATTERN: Pattern = Pattern.compile(
    "^(?=.*[0-9])(?=.*\\p{Lu})(?=.*\\p{Ll})(?=.*[!-/:-@-`{-~])(?=\\S+$).{8,}$"
)

var DEVICES = emptyList<DeviceModel>()
var LOCALIZATIONS = LocalizationsModel(
    logs = emptyList(),
    approvalRequestTypes = emptyList(),
    errors = emptyList()
)

lateinit var ENVIRONMENT: ApplicationEnvironment
lateinit var APPLICATION_LANGUAGE: ApplicationLanguage

const val BG_COUNTRY_CODE = "+359"
const val DEFAULT_INACTIVITY_TIMEOUT_MILLISECONDS = 120000L
const val CHECK_STATUS_INTERVAL_DELAY = 10000L
const val SIGNING_REQUEST_TIMEOUT = 270000L
const val FILTER_MODEL_KEY = "FILTER_MODEL_KEY"
const val FILTER_MODEL_REQUEST_KEY = "FILTER_MODEL_REQUEST_KEY"
const val REFRESH_CERTIFICATES_REQUEST_KEY = "REFRESH_CERTIFICATES_REQUEST_KEY"
const val REFRESH_CERTIFICATES_KEY = "REFRESH_CERTIFICATES_KEY"
const val REFRESH_APPLICATIONS_REQUEST_KEY = "REFRESH_APPLICATIONS_REQUEST_KEY"
const val REFRESH_APPLICATIONS_KEY = "REFRESH_APPLICATIONS_KEY"
const val REFRESH_CITIZEN_INFORMATION_REQUEST_KEY = "REFRESH_CITIZEN_INFORMATION_REQUEST_KEY"
const val REFRESH_CITIZEN_INFORMATION_KEY = "REFRESH_CITIZEN_INFORMATION_KEY"
const val NAMES_MIN_LENGTH = 3
const val NAMES_MAX_LENGTH = 35
const val OTP_CODE_LENGTH = 8
const val MIN_AGE_YEARS = 18
const val PIN_MAX_LENGTH = 6
const val CERTIFICATE_ALIAS_MAX_LENGTH = 30

enum class TimeZones(
    override val type: String,
) : TypeEnum {
    UTC("UTC"),
    BULGARIA("GMT+02:00"),
    DEFAULT(TimeZone.getDefault().id)
}

enum class UiDateFormats(
    override val type: String,
) : TypeEnum {
    WITHOUT_TIME("yyyy-MM-dd"),
    WITH_TIME("yyyy-MM-dd HH:mm:ss"),
    WITH_TIME_SLASH("yyyy-MM-dd_HH/mm/ss"),
}

// ATTENTION, first there must be more complex formats, and then simpler ones,
// otherwise during parsing only the initial part of the date will be determined
enum class FromServerDateFormats(
    override val type: String,
) : TypeEnum {
    WITH_MILLIS("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'"),
    WITHOUT_MILLIS("yyyy-MM-dd'T'HH:mm:ss'Z'"),
    ONLY_DATE("yyyy-MM-dd"),
}

enum class ToServerDateFormats(
    override val type: String,
) : TypeEnum {
    ONLY_DATE("yyyy-MM-dd"),
    WITH_SECONDS("yyyy-MM-dd'T'HH:mm:ss'Z'"),
    WITHOUT_TIME_ZONE("yyyy-MM-dd'T'HH:mm:ss"),
    WITH_MILLIS("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'"),
}

// USERID 00000000-0000-0000-0000-008709299269
// userid 4e6f4263-8c80-4726-9409-a8cb73fef471

// put(
//        key = "",
//        value = MockResponse(
//            isEnabled = false,
//            body = "",
//            message = "",
//            serverCode = 200,
//        )
//    )

val mockResponses = mutableMapOf<String, MockResponse>()
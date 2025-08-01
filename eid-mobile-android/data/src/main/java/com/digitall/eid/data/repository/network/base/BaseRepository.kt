/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.repository.network.base

import com.digitall.eid.data.BuildConfig.CLIENT_ID
import com.digitall.eid.data.BuildConfig.CLIENT_SECRET_AUTH
import com.digitall.eid.data.BuildConfig.GRANT_TYPE_AUTH
import com.digitall.eid.data.di.OKHTTP
import com.digitall.eid.data.extensions.isJSONValid
import com.digitall.eid.data.extensions.toCamelCase
import com.digitall.eid.data.models.network.base.EmptyResponse
import com.digitall.eid.data.models.network.base.ErrorApiResponse
import com.digitall.eid.data.models.network.base.ErrorResponse
import com.digitall.eid.data.models.network.base.getEmptyResponse
import com.digitall.eid.data.utils.CoroutineContextProvider
import com.digitall.eid.domain.ENVIRONMENT
import com.digitall.eid.domain.LOCALIZATIONS
import com.digitall.eid.domain.models.assets.localization.errors.ErrorLocalizationModel
import com.digitall.eid.domain.models.base.ErrorType
import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.models.common.ApplicationInfo
import com.digitall.eid.domain.repository.common.PreferencesRepository
import com.digitall.eid.domain.utils.JWTDecoder
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.google.gson.Gson
import com.google.gson.JsonSyntaxException
import okhttp3.FormBody
import okhttp3.OkHttpClient
import okhttp3.RequestBody
import org.json.JSONObject
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject
import org.koin.core.qualifier.named
import retrofit2.Response
import java.net.UnknownHostException
import javax.net.ssl.SSLPeerUnverifiedException

abstract class BaseRepository : KoinComponent {

    companion object {
        private const val TAG = "BaseRepositoryTag"
    }

    // TODO network info from error view must be removed

    private val gson: Gson by inject()
    private val jWTDecoder: JWTDecoder by inject()
    private val preferences: PreferencesRepository by inject()
    private val client: OkHttpClient by inject(named(OKHTTP))
    private val contextProvider: CoroutineContextProvider by inject()

    private val errors: List<ErrorLocalizationModel>
        get() = LOCALIZATIONS.errors

    protected suspend fun <T> getResult(
        call: suspend () -> Response<T>,
    ): ResultEmittedData<T> {
        return getResultLoop(call)
    }

    private suspend fun <T> getResultLoop(
        call: suspend () -> Response<T>,
        retryCount: Int = 0,
    ): ResultEmittedData<T> {
        return try {
            val response = call()
            val responseCode = response.code()
            val successCode = when (responseCode) {
                200,
                201,
                202,
                203,
                204,
                205,
                206,
                207,
                208,
                226 -> true

                else -> false
            }
            val responseMessage = response.message()
            val responseBody = response.body() ?: getEmptyResponse()
            when {
                successCode && responseBody !is EmptyResponse -> {
                    dataSuccess(
                        model = responseBody,
                        message = responseMessage,
                        responseCode = responseCode,
                    )
                }

                successCode -> {
                    logDebug("getResult successCode", TAG)
                    dataSuccess(
                        model = getEmptyResponse(),
                        message = responseMessage,
                        responseCode = responseCode,
                    )
                }

                responseCode == 401 -> {
                    val errorApiResponse = parseErrorApi(response)
                    val messageKey = when (errorApiResponse?.errors) {
                        is Map<*, *> -> errorApiResponse.errors.keys.first() as? String
                        is List<*> -> errorApiResponse.errors.first() as? String
                        else -> if (responseMessage.isNullOrEmpty()
                                .not()
                        ) responseMessage else "Error while receiving data from server, error code: $responseCode"

                    }
                    val message =
                        errors.firstOrNull { error -> error.type == messageKey?.toCamelCase() }?.description
                    logError(
                        "getResult responseCode == 401, message: $message",
                        TAG
                    )
                    dataError(
                        message = message,
                        model = responseBody,
                        error = errorApiResponse,
                        responseCode = responseCode,
                        title = "Authorization error",
                        errorType = ErrorType.AUTHORIZATION,
                    )
                }

                else -> {
                    val errorApiResponse = parseErrorApi(response)
                    val title = when {
                        errorApiResponse != null && !errorApiResponse.title.isNullOrEmpty() -> errorApiResponse.title
                        else -> "Server error"
                    }
                    val messageKey = when (errorApiResponse?.errors) {
                        is Map<*, *> -> errorApiResponse.errors.keys.first() as String
                        is List<*> -> errorApiResponse.errors.first() as String
                        else -> if (responseMessage.isNullOrEmpty()
                                .not()
                        ) responseMessage else "Error while receiving data from server, error code: $responseCode"
                    }
                    val message =
                        errors.firstOrNull { error -> error.type == messageKey.toCamelCase() }?.description
                    logError("getResult conditions else errorApiResponse: $errorApiResponse", TAG)
                    dataError(
                        title = title,
                        message = message,
                        model = responseBody,
                        error = errorApiResponse,
                        responseCode = responseCode,
                        errorType = ErrorType.SERVER_ERROR,
                    )
                }
            }
        } catch (exception: UnknownHostException) {
            logError(
                "getResult Exception is UnknownHostException, message: ${exception.message} stackTrace: ${exception.stackTrace}",
                exception,
                TAG
            )
            dataError(
                model = null,
                error = null,
                responseCode = null,
                title = "No internet connection",
                errorType = ErrorType.NO_INTERNET_CONNECTION,
                message = "Connect to the Internet and try again",
            )
        } catch (exception: SSLPeerUnverifiedException) {
            logError(
                "getResult Exception is SSLPeerUnverifiedException, message: ${exception.message} stackTrace: ${exception.stackTrace}\",",
                exception,
                TAG
            )
            dataError(
                model = null,
                error = null,
                responseCode = null,
                title = "Encryption error",
                errorType = ErrorType.EXCEPTION,
                message = "Encryption error when receiving data from the server, contact your service provider",
            )
        } catch (exception: JsonSyntaxException) {
            logError(
                "getResult Exception is JsonSyntaxException, message: ${exception.message} stackTrace: ${exception.stackTrace}\",",
                exception,
                TAG
            )
            dataError(
                model = null,
                error = null,
                responseCode = null,
                title = "Server error",
                errorType = ErrorType.EXCEPTION,
                message = "Error while receiving data from server, data format incorrect",
            )
        } catch (exception: java.io.EOFException) {
            logError(
                "getResult Exception is EOFException, message: ${exception.message} stackTrace: ${exception.stackTrace}\",",
                exception,
                TAG
            )
            dataError(
                model = null,
                error = null,
                responseCode = null,
                title = "Server error",
                errorType = ErrorType.EXCEPTION,
                message = exception.message ?: exception.toString(),
            )
        } catch (exception: Throwable) {
            logError(
                "getResult Exception is other, message: ${exception.message} stackTrace: ${exception.stackTrace}\",",
                exception,
                TAG
            )
            dataError(
                model = null,
                error = null,
                responseCode = null,
                title = "Server error",
                errorType = ErrorType.EXCEPTION,
                message = exception.message ?: exception.toString(),
            )
        }
    }

    private fun <T> parseErrorApi(response: Response<T>): ErrorApiResponse? {
        return try {
            val responseBodyString = response.errorBody()?.string()
            if (responseBodyString?.isJSONValid() == true) {
                gson.fromJson(responseBodyString, ErrorApiResponse::class.java)
            } else {
                ErrorApiResponse(
                    status = response.code(),
                    title = response.message(),
                    detail = null,
                    errors = null,
                )
            }
        } catch (e: Exception) {
            logError("parseErrorApi Exception: ${e.message}", e, TAG)
            null
        }
    }


    private fun <T> dataError(
        model: T?,
        error: ErrorResponse?,
        responseCode: Int?,
        title: String?,
        message: String?,
        errorType: ErrorType?,
    ): ResultEmittedData<T> = ResultEmittedData.error(
        model = model,
        error = error,
        title = title,
        message = message,
        errorType = errorType,
        responseCode = responseCode,
    )

    private fun <T> dataSuccess(
        model: T,
        message: String?,
        responseCode: Int,
    ): ResultEmittedData<T> = ResultEmittedData.success(
        model = model,
        message = message,
        responseCode = responseCode,
    )


    private fun refreshToken(
        applicationInfo: ApplicationInfo,
    ): Boolean {
        logDebug("refresh token", TAG)
        val refreshToken = preferences.readApplicationInfo()?.refreshToken
        if (refreshToken.isNullOrEmpty()) {
            logError("refresh token not isSuccessful, old refreshToken is null or empty", TAG)
            return false
        }
        try {
            val requestBody: RequestBody = FormBody.Builder()
                .add("refresh_token", refreshToken)
                .build()
            val request = okhttp3.Request.Builder()
                .url(ENVIRONMENT.urlKeycloakPg + "realms/eid_public/protocol/openid-connect/token")
                .post(requestBody)
                .build()
            val response = client.newCall(request)
                .execute()
            val responseBody = response.body
            val responseCode = response.code
            val successCode = when (responseCode) {
                200,
                201,
                202,
                203,
                204,
                205,
                206,
                207,
                208,
                226 -> true

                else -> false
            }
            if (!response.isSuccessful || !successCode) {
                logError(
                    "refresh token not successful, response code: ${response.code}",
                    TAG
                )
                return false
            }
            val bodyString = responseBody.string()
            if (bodyString.isEmpty()) {
                logError("refresh token not successful, body string is empty", TAG)
                return false
            }
            val objectBody = JSONObject(bodyString)
            val accessToken = objectBody.getString("access_token")
            if (accessToken.isNullOrEmpty()) {
                logError("refresh token not successful, access token is null or empty", TAG)
                return false
            }
            val user = jWTDecoder.getUser(accessToken)
            if (user == null) {
                logError("refresh token not successful, user from jWT == null", TAG)
                return false
            }
            preferences.saveApplicationInfo(
                applicationInfo.copy(
                    userModel = user,
                    accessToken = accessToken,
                )
            )
            logDebug("refresh token ready", TAG)
            return true
        } catch (e: Exception) {
            logError("refresh token exception: ${e.message}", e, TAG)
            return false
        }
    }

    private fun enterToAccount(
        applicationInfo: ApplicationInfo,
    ): Boolean {
        logDebug("enter to account", TAG)
        val email = applicationInfo.email
        if (email.isEmpty()) {
            logError("enter to account is not successful, email is empty", TAG)
            return false
        }
        val password = applicationInfo.password
        if (password.isEmpty()) {
            logError("enter to account is not successful, password is empty", TAG)
            return false
        }
        try {
            val requestBody: RequestBody = FormBody.Builder()
                .add("username", email)
                .add("password", password)
                .add("grant_type", GRANT_TYPE_AUTH)
                .add("client_secret", CLIENT_SECRET_AUTH)
                .add("client_id", CLIENT_ID)
                .build()
            val request = okhttp3.Request.Builder()
                .url(ENVIRONMENT.urlKeycloakPg + "realms/eid_public/protocol/openid-connect/token")
                .post(requestBody)
                .build()
            val response = client.newCall(request)
                .execute()
            val responseBody = response.body
            val responseCode = response.code
            val successCode = when (responseCode) {
                200,
                201,
                202,
                203,
                204,
                205,
                206,
                207,
                208,
                226 -> true

                else -> false
            }
            if (!response.isSuccessful || !successCode) {
                logError("enter to account is not successful, response code: ${response.code}", TAG)
                return false
            }
            val bodyString = responseBody.string()
            if (bodyString.isEmpty()) {
                logError("enter to account is not successful, body string is empty", TAG)
                return false
            }
            val objectBody = JSONObject(bodyString)
            val accessToken = objectBody.getString("access_token")
            if (accessToken.isNullOrEmpty()) {
                logError("enter to account is not successful, accessToken is null or empty", TAG)
                return false
            }
            val refreshToken = objectBody.getString("refresh_token")
            if (refreshToken.isNullOrEmpty()) {
                logError("enter to account is not successful, refreshToken is null or empty", TAG)
                return false
            }
            val user = jWTDecoder.getUser(accessToken)
            if (user == null) {
                logError("refresh token not successful, user from jWT == null", TAG)
                return false
            }
            preferences.saveApplicationInfo(
                applicationInfo.copy(
                    userModel = user,
                    accessToken = accessToken,
                    refreshToken = refreshToken,
                )
            )
            logDebug("enter to account ready", TAG)
            return true
        } catch (e: Exception) {
            logError("enter to account exception: ${e.message}", e, TAG)
            return false
        }
    }

}
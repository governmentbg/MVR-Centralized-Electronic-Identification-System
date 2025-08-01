/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.network.utils

import com.digitall.eid.domain.repository.common.PreferencesRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject

class HeaderInterceptor : okhttp3.Interceptor,
    KoinComponent {

    companion object {
        private const val TAG = "HeaderInterceptorTag"
    }

    private val preferences: PreferencesRepository by inject()

    override fun intercept(chain: okhttp3.Interceptor.Chain): okhttp3.Response {
        logDebug("intercept", TAG)
        val original = chain.request()
        val request = original.newBuilder()
            .header("Content-Type", "application/json")
            .header("Cookie", "KEYCLOAK_LOCALE=bg")
            .method(original.method, original.body)
        val token = preferences.readApplicationInfo()?.accessToken
        if (!token.isNullOrEmpty()) {
            logDebug("add token: $token", TAG)
            request.header("Authorization", "Bearer $token")
        }
        return chain.proceed(request.build())
    }
}
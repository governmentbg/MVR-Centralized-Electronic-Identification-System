/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.usecase.logout

import com.digitall.eid.domain.models.base.ResultEmittedData
import com.digitall.eid.domain.repository.common.PreferencesRepository
import com.digitall.eid.domain.usecase.base.BaseUseCase
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.flow
import kotlinx.coroutines.flow.flowOn
import org.koin.core.component.inject

/** imports for flow:
 * import kotlinx.coroutines.flow.Flow
 * import kotlinx.coroutines.flow.collect
 * import kotlinx.coroutines.flow.flow
 * import kotlinx.coroutines.flow.onEach
 **/

class LogoutUseCase : BaseUseCase {

    private val preferences: PreferencesRepository by inject()

    fun invoke(): Flow<ResultEmittedData<Unit>> = flow {
        emit(ResultEmittedData.loading(model = null))
//        preferences.logoutFromPreferences()
        emit(
            ResultEmittedData.success(
                model = Unit,
                message = null,
                responseCode = null,
            )
        )
    }.flowOn(Dispatchers.IO)

}
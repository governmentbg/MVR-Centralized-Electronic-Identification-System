/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.domain.repository.network.applications

import com.digitall.eid.domain.models.applications.create.ApplicationConfirmWithBaseProfileRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationConfirmWithEIDRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationEnrollWithBaseProfileRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationEnrollWithBaseProfileResponseModel
import com.digitall.eid.domain.models.applications.create.ApplicationEnrollWithEIDRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationEnrollWithEIDResponseModel
import com.digitall.eid.domain.models.applications.create.ApplicationGenerateUserDetailsXMLResponseModel
import com.digitall.eid.domain.models.applications.create.ApplicationSendSignatureRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationSendSignatureResponseModel
import com.digitall.eid.domain.models.applications.create.ApplicationSignWithBaseProfileRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationUpdateProfileRequestModel
import com.digitall.eid.domain.models.applications.create.ApplicationUpdateProfileResponseModel
import com.digitall.eid.domain.models.applications.create.ApplicationUserDetailsModel
import com.digitall.eid.domain.models.applications.create.ApplicationDetailsXMLRequestModel
import com.digitall.eid.domain.models.base.ResultEmittedData
import kotlinx.coroutines.flow.Flow

interface ApplicationCreateNetworkRepository {

    fun getUserDetails(): Flow<ResultEmittedData<ApplicationUserDetailsModel>>

    fun generateUserDetailsXML(
        data: ApplicationDetailsXMLRequestModel,
    ): Flow<ResultEmittedData<ApplicationGenerateUserDetailsXMLResponseModel>>

    fun sendSignature(
        data: ApplicationSendSignatureRequestModel,
    ): Flow<ResultEmittedData<ApplicationSendSignatureResponseModel>>

    fun sendCertificateApplication(
        data: ApplicationSendSignatureRequestModel,
    ): Flow<ResultEmittedData<ApplicationSendSignatureResponseModel>>

    fun signWithBaseProfile(
        data: ApplicationSignWithBaseProfileRequestModel,
    ): Flow<ResultEmittedData<String>>

    fun enrollWithBaseProfile(
        data: ApplicationEnrollWithBaseProfileRequestModel,
    ): Flow<ResultEmittedData<ApplicationEnrollWithBaseProfileResponseModel>>

    fun confirmWithBaseProfile(
        data: ApplicationConfirmWithBaseProfileRequestModel,
    ): Flow<ResultEmittedData<String>>

    fun enrollWithEID(
        data: ApplicationEnrollWithEIDRequestModel,
    ): Flow<ResultEmittedData<ApplicationEnrollWithEIDResponseModel>>

    fun confirmWithEID(
        data: ApplicationConfirmWithEIDRequestModel,
    ): Flow<ResultEmittedData<String>>

    fun updateProfile(
        data: ApplicationUpdateProfileRequestModel
    ): Flow<ResultEmittedData<ApplicationUpdateProfileResponseModel>>

}
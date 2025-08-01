package com.digitall.eid.domain.repository.network.assets

import com.digitall.eid.domain.models.assets.localization.LocalizationsModel
import com.digitall.eid.domain.models.base.ResultEmittedData
import kotlinx.coroutines.flow.Flow

interface AssetsNetworkRepository {

    fun getLocalizations(language: String): Flow<ResultEmittedData<LocalizationsModel>>

}
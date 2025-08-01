package com.digitall.eid.models.common

sealed class CardScanScreenStates {
    data object Scanning : CardScanScreenStates()
    data object Processing : CardScanScreenStates()
    data class Success(val result: CardScanResult) : CardScanScreenStates()
    data class Error(val message: StringSource) : CardScanScreenStates()
    data object Suspended : CardScanScreenStates()
}

sealed class CardScanResult {
    data class ChallengeSigned(
        val signature: String,
        val challenge: String,
        val certificate: String,
        val certificateChain: List<String>
    ) : CardScanResult()

    data object PinChanged : CardScanResult()
}
/**
 * domStorageEnabled for http
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 */
package com.digitall.eid.ui.fragments.base

import android.annotation.SuppressLint
import android.graphics.Bitmap
import android.net.http.SslError
import android.view.View
import android.webkit.SslErrorHandler
import android.webkit.WebChromeClient
import android.webkit.WebResourceError
import android.webkit.WebResourceRequest
import android.webkit.WebView
import android.webkit.WebViewClient
import androidx.annotation.CallSuper
import com.digitall.eid.databinding.FragmentBaseWebViewWithActionButtonBinding
import com.digitall.eid.domain.DELAY_1000
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.callPhoneNumber
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.sendMail
import com.digitall.eid.extensions.setTextResource
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.BaseViewModel

abstract class BaseWebViewFragment<VM : BaseViewModel> :
    BaseFragment<FragmentBaseWebViewWithActionButtonBinding, VM>() {

    companion object {
        private const val TAG = "BaseWebViewFragmentTag"
        private const val TEL_SCHEME = "tel:"
        private const val EMAIL_SCHEME = "mailto:"
    }

    override fun getViewBinding() =
        FragmentBaseWebViewWithActionButtonBinding.inflate(layoutInflater)

    protected abstract val showToolbar: Boolean

    protected abstract val showSettingsButton: Boolean

    protected abstract val toolbarNavigationIconRes: Int?

    protected abstract val toolbarNavigationTextRes: Int?

    open val fabButtonText: Int? = null

    @CallSuper
    @SuppressLint("SetJavaScriptEnabled")
    override fun setupView() {
        logDebug("setupView", TAG)
        binding.webView.apply {
            webViewClient = CustomWebViewClient()
            webChromeClient = WebChromeClient()
            settings.apply {
                setSupportZoom(true)
                builtInZoomControls = true
                displayZoomControls = false
                javaScriptEnabled = true
                useWideViewPort = true
                loadWithOverviewMode = true
                domStorageEnabled = true
            }
            setInitialScale(1)
            isVerticalScrollBarEnabled = false
        }
        if (showToolbar) {
            binding.customToolbar.visibility = View.VISIBLE
            if (showSettingsButton) {
                binding.customToolbar.setSettingsIcon(
                    settingsClickListener = {

                    }
                )
            }
            toolbarNavigationIconRes?.let {
                binding.customToolbar.setNavigationIcon(
                    navigationIconRes = it,
                    navigationClickListener = {
                        viewModel.onBackPressed()
                    }
                )
            }
            toolbarNavigationTextRes?.let {
                binding.customToolbar.setTitleText(StringSource(it))
            }
        } else {
            binding.customToolbar.visibility = View.GONE
        }

        fabButtonText?.let {
            binding.multilineFab.tvFabText.setTextResource(it)
        }
    }

    @CallSuper
    override fun setupControls() {
        binding.refreshLayout.setOnRefreshListener(binding.webView::reload)
        binding.multilineFab.btnFab.onClickThrottle {
            viewModel.citizenAssociateEID()
        }
    }

    open fun needToLoadPage(url: String?): Boolean = true

    protected fun showActionButton(isVisible: Boolean) {
        binding.multilineFab.root.visibility =
            if (isVisible) View.VISIBLE else View.GONE
    }

    protected fun loadWebPage(pageUrl: String, headers: Map<String, String>? = null) {
        logDebug("loadWebPage pageUrl: $pageUrl", TAG)
        if (headers.isNullOrEmpty()) {
            binding.webView.loadUrl(pageUrl)
        } else {
            binding.webView.loadUrl(pageUrl, headers)
        }
    }

    @CallSuper
    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        if (binding.webView.canGoBack()) binding.webView.goBack()
        else viewModel.onBackPressed()
    }

    @CallSuper
    override fun onResumed() {
        binding.webView.onResume()
    }

    @CallSuper
    override fun onPaused() {
        binding.webView.onPause()
    }

    @CallSuper
    override fun onDestroyed() {
        try {
            binding.webView.removeAllViews()
            binding.webView.destroy()
        } catch (e: Exception) {
            /* do nothing */
        }
    }

    inner class CustomWebViewClient : WebViewClient() {

        override fun shouldOverrideUrlLoading(
            view: WebView?,
            request: WebResourceRequest?
        ): Boolean {
            logDebug("shouldOverrideUrlLoading", TAG)
            val requestUrl = request?.url.toString()
            return when {
                requestUrl.startsWith(TEL_SCHEME) -> {
                    val phone = requestUrl.removePrefix(TEL_SCHEME)
                    requireContext().callPhoneNumber(phone)
                    true
                }

                requestUrl.startsWith(EMAIL_SCHEME) -> {
                    val email = requestUrl.removePrefix(EMAIL_SCHEME)
                    requireContext().sendMail(email)
                    true
                }

                else -> !needToLoadPage(request?.url.toString())
            }
        }

        override fun onPageStarted(view: WebView?, url: String?, favicon: Bitmap?) {
            logDebug("onPageStarted", TAG)
            binding.refreshLayout.isRefreshing = false
            viewModel.showLoader()
        }

        override fun onPageFinished(view: WebView, url: String) {
            logDebug("onPageFinished", TAG)
            binding.refreshLayout.isRefreshing = false
            viewModel.hideLoader(delay = DELAY_1000)
        }

        override fun onReceivedError(
            view: WebView?,
            request: WebResourceRequest?,
            error: WebResourceError?,
        ) {
            super.onReceivedError(view, request, error)
            logError("onReceivedError", TAG)
            viewModel.hideLoader(delay = DELAY_1000)
        }

        override fun onReceivedSslError(
            view: WebView?,
            handler: SslErrorHandler?,
            error: SslError?
        ) {
            // WARNING: This is enabled only for testing
            handler?.proceed()
        }
    }
}
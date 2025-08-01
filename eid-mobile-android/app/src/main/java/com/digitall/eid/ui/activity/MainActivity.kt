/**
 * Use single activity
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 *
 * Activity lifecycle:
 *
 * onCreate(savedInstanceState: Bundle?)
 * onStart()
 * onRestart()
 * onResume()
 *
 * onPause()
 * onStop()
 * onDestroy()
 *
 * Fragment lifecycle:
 *
 * onAttach(context: Context)
 * onCreate()
 * onCreateView()
 * onViewCreated()
 * onViewStateRestored(savedInstanceState: Bundle?)
 * onStart()
 * onResume()
 *
 * onPause()
 * onStop()
 * onDestroyView()
 * onDestroy()
 * onDetach()
 *
 * View lifecycle:
 *
 * onAttachedToWindow()
 * requestLayout()
 * measure()
 * onMeasure()
 * layout()
 * onLayout()
 * invalidate()
 * dispatchToDraw()
 * draw()
 * onDraw()
 *
 */
package com.digitall.eid.ui.activity

import android.Manifest
import android.app.Dialog
import android.content.ComponentCallbacks2
import android.content.Context
import android.content.DialogInterface
import android.content.pm.PackageManager
import android.os.Build
import android.os.Bundle
import android.view.MotionEvent
import android.view.View
import androidx.annotation.CallSuper
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import androidx.core.app.ActivityCompat
import androidx.core.content.ContextCompat
import androidx.core.splashscreen.SplashScreen.Companion.installSplashScreen
import androidx.core.view.WindowCompat
import androidx.navigation.fragment.NavHostFragment
import com.digitall.eid.BuildConfig
import com.digitall.eid.R
import com.digitall.eid.databinding.ActivityBaseBinding
import com.digitall.eid.domain.repository.common.PreferencesRepository
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.hideKeyboard
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.BannerMessage
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.MessageBannerHolder
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.view.FullscreenLoaderView
import com.digitall.eid.utils.AlertDialogResultListener
import com.digitall.eid.utils.AppUncaughtExceptionHandler
import com.digitall.eid.utils.BannerMessageWindowManager
import com.digitall.eid.utils.CurrentContext
import com.digitall.eid.utils.InactivityTimer
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class MainActivity : AppCompatActivity(),
    MessageBannerHolder,
    ComponentCallbacks2 {

    companion object {
        private const val TAG = "BaseActivityTag"
    }

    private val viewModel: MainViewModel by viewModel()
    private val appContext: Context by inject()
    private val inactivityTimer: InactivityTimer by inject()
    private val currentContext: CurrentContext by inject()
    private val preferences: PreferencesRepository by inject()

    private fun getStartDestination(): StartDestination {
        return viewModel.getStartDestination(intent)
    }

    lateinit var binding: ActivityBaseBinding

    private lateinit var bannerMessageWindowManager: BannerMessageWindowManager

    var alertDialogResultListener: AlertDialogResultListener? = null

    private var fullscreenLoaderView: FullscreenLoaderView? = null

    private var messageDialog: Dialog? = null

    @CallSuper
    override fun onCreate(savedInstanceState: Bundle?) {
        installSplashScreen()
        super.onCreate(savedInstanceState)
        Thread.setDefaultUncaughtExceptionHandler(AppUncaughtExceptionHandler())
        viewModel.applyLightDarkTheme()
        currentContext.attachBaseContext(this)
        binding = ActivityBaseBinding.inflate(layoutInflater)
        setContentView(binding.root)
        bannerMessageWindowManager = BannerMessageWindowManager(this)
        preferences.logoutFromPreferencesFully()
        setupNavController()
        subscribeToLiveData()
        viewModel.onViewCreated()
        WindowCompat.setDecorFitsSystemWindows(window, false)
        if (BuildConfig.DEBUG && Build.VERSION.SDK_INT <= 29 &&
            ContextCompat.checkSelfPermission(
                this,
                Manifest.permission.WRITE_EXTERNAL_STORAGE
            ) != PackageManager.PERMISSION_GRANTED
        ) {
            ActivityCompat.requestPermissions(
                this,
                arrayOf(Manifest.permission.WRITE_EXTERNAL_STORAGE),
                100
            )
        }
        // TODO uncomment for disable screenshots
        //        window.setFlags(
        //            WindowManager.LayoutParams.FLAG_SECURE,
        //            WindowManager.LayoutParams.FLAG_SECURE
        //        )
    }

    override fun dispatchTouchEvent(event: MotionEvent): Boolean {
        if (event.action == MotionEvent.ACTION_DOWN) {
            viewModel.dispatchTouchEvent()
        }
        return super.dispatchTouchEvent(event)
    }

    private fun setupNavController() {
        val host =
            supportFragmentManager.findFragmentById(R.id.navigationContainer) as NavHostFragment
        try {
            // Try to get the current graph, if it is there, nav controller is valid.
            // When there is no graph, it throws IllegalStateException,
            // then we need to create a graph ourselves
            host.navController.graph
        } catch (e: Exception) {
            val graphInflater = host.navController.navInflater
            val graph = graphInflater.inflate(R.navigation.nav_activity)
            val startDestination = getStartDestination()
            graph.setStartDestination(startDestination.destination)
            host.navController.setGraph(graph, startDestination.arguments)
        }
        viewModel.bindActivityNavController(host.navController)
    }

    private fun subscribeToLiveData() {
        viewModel.closeActivityLiveData.observe(this) {
            finish()
        }
        viewModel.showBannerMessageLiveData.observe(this) {
            showMessage(it)
        }
        viewModel.showDialogMessageLiveData.observe(this) {
            showMessage(it)
        }
        inactivityTimer.lockStatusLiveData.observe(this) {
            if (it) {
                logDebug("lockStatusLiveData onLoginTimerExpired", TAG)
                messageDialog?.dismiss()
                viewModel.toLoginFragment()
            }
        }
    }


    override fun showMessage(message: BannerMessage, anchorView: View?) {
        logDebug("showMessage message: ${message.message.getString(this)}", TAG)
        try {
            bannerMessageWindowManager.showMessage(
                bannerMessage = message,
                anchorView = anchorView ?: binding.rootLayout,
            )
        } catch (e: Exception) {
            logError("showBannerMessage Exception: ${e.message}", e, TAG)
        }
    }

    override fun showMessage(message: DialogMessage) {
        val builder = AlertDialog.Builder(this)
            .setMessage(message.message.getString(this))
        if (message.title != null) {
            builder.setTitle(message.title.getString(this))
        }
        if (message.positiveButtonText != null) {
            builder.setPositiveButton(message.positiveButtonText.getString(this)) { dialog, _ ->
                logDebug("alertDialog result positive", TAG)
                dialog.dismiss()
                alertDialogResultListener?.onAlertDialogResultReady(
                    AlertDialogResult(
                        messageId = message.messageID,
                        isPositive = true,
                    )
                )
            }
        }
        if (message.negativeButtonText != null) {
            builder.setNegativeButton(message.negativeButtonText.getString(this)) { dialog, _ ->
                logDebug("alertDialog result negative", TAG)
                dialog.dismiss()
                alertDialogResultListener?.onAlertDialogResultReady(
                    AlertDialogResult(
                        messageId = message.messageID,
                        isPositive = false,
                    )
                )
            }
        }
        builder.setOnCancelListener { _: DialogInterface? ->
            logDebug("alertDialog result negative", TAG)
            alertDialogResultListener?.onAlertDialogResultReady(
                AlertDialogResult(
                    messageId = message.messageID,
                    isPositive = false,
                )
            )
        }
        messageDialog = builder.create().also { dialog ->
            dialog.setCancelable(false)
            dialog.setCanceledOnTouchOutside(false)
            dialog.show()
        }
    }

    override fun showFullscreenLoader(message: StringSource?) {
        fullscreenLoaderView?.let { loader ->
            message?.let {
                loader.setMessage(message = it)
            }
        } ?: run {
            fullscreenLoaderView = FullscreenLoaderView(this).also { loader ->
                message?.let {
                    loader.setMessage(message = it)
                }
                loader.show()
            }
        }
    }

    override fun hideFullscreenLoader() {
        fullscreenLoaderView = try {
            fullscreenLoaderView?.dismiss()
            null
        } catch (exception: Exception) {
            logError("Hiding fullscreen loader returned an exception: ${exception.message}", TAG)
            null
        }
    }

    override fun onResume() {
        logDebug("onResume", TAG)
        super.onResume()
        viewModel.onResume()
    }

    @CallSuper
    override fun onPause() {
        logDebug("onPause", TAG)
        hideKeyboard()
        super.onPause()
        viewModel.onPause()
    }

    @CallSuper
    override fun onDestroy() {
        logDebug("onDestroy", TAG)
        super.onDestroy()
        viewModel.onDestroy()
        bannerMessageWindowManager.hideWindow()
        // Reset this activity context
        if (currentContext.get() == this) {
            currentContext.attachBaseContext(appContext)
        }
    }
}
/**
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
package com.digitall.eid.ui.fragments.base

import android.content.Context
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.FrameLayout
import android.widget.ListAdapter
import android.widget.PopupWindow
import androidx.activity.OnBackPressedCallback
import androidx.annotation.CallSuper
import androidx.appcompat.widget.AppCompatButton
import androidx.appcompat.widget.AppCompatImageView
import androidx.appcompat.widget.AppCompatTextView
import androidx.appcompat.widget.ListPopupWindow
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import androidx.core.view.isVisible
import androidx.fragment.app.Fragment
import androidx.lifecycle.lifecycleScope
import androidx.navigation.fragment.NavHostFragment
import androidx.navigation.fragment.findNavController
import androidx.viewbinding.ViewBinding
import com.digitall.eid.R
import com.digitall.eid.data.extensions.getParcelableCompat
import com.digitall.eid.domain.extensions.getCalendar
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.findActivityNavController
import com.digitall.eid.extensions.findParentFragmentByType
import com.digitall.eid.extensions.findParentFragmentResultListenerFragmentManager
import com.digitall.eid.extensions.hideKeyboard
import com.digitall.eid.extensions.setBackgroundColorResource
import com.digitall.eid.extensions.showDataPicker
import com.digitall.eid.extensions.showSpinner
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.BannerMessage
import com.digitall.eid.models.common.DatePickerConfig
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.ErrorState
import com.digitall.eid.models.common.FullscreenLoadingState
import com.digitall.eid.models.common.LoadingState
import com.digitall.eid.models.common.MessageBannerHolder
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.common.UiState
import com.digitall.eid.models.list.CommonDatePickerUi
import com.digitall.eid.models.list.CommonDialogWithSearchMultiselectUi
import com.digitall.eid.models.list.CommonDialogWithSearchUi
import com.digitall.eid.models.list.CommonSpinnerUi
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.ui.activity.MainActivity
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import com.digitall.eid.ui.fragments.common.search.multiselect.CommonBottomSheetWithSearchMultiselectFragment.Companion.COMMON_BOTTOM_SHEET_WITH_SEARCH_MULTISELECT_FRAGMENT_RESULT_BUNDLE_KEY
import com.digitall.eid.ui.fragments.common.search.multiselect.CommonBottomSheetWithSearchMultiselectFragment.Companion.COMMON_BOTTOM_SHEET_WITH_SEARCH_MULTISELECT_FRAGMENT_RESULT_DATA_KEY
import com.digitall.eid.ui.fragments.common.search.normal.CommonBottomSheetWithSearchFragment.Companion.COMMON_BOTTOM_SHEET_WITH_SEARCH_FRAGMENT_RESULT_BUNDLE_KEY
import com.digitall.eid.ui.fragments.common.search.normal.CommonBottomSheetWithSearchFragment.Companion.COMMON_BOTTOM_SHEET_WITH_SEARCH_FRAGMENT_RESULT_DATA_KEY
import com.digitall.eid.ui.fragments.main.flow.MainTabsFlowFragment
import com.digitall.eid.ui.view.ComplexGestureRefreshView
import com.digitall.eid.utils.AlertDialogResultListener
import com.digitall.eid.utils.SoftKeyboardStateWatcher
import kotlinx.coroutines.Job
import kotlinx.coroutines.launch

abstract class BaseFragment<VB : ViewBinding, VM : BaseViewModel> : Fragment(),
    MessageBannerHolder,
    AlertDialogResultListener {

    companion object {
        private const val TAG = "BaseFragmentTag"
        const val DIALOG_EXIT = "DIALOG_EXIT"
    }

    abstract val viewModel: VM

    private var viewBinding: VB? = null

    private var popupWindow: PopupWindow? = null

    private var listPopupWindow: ListPopupWindow? = null

    // This property is only valid between onCreateView and
    // onDestroyView.
    val binding get() = viewBinding!!

    private var keyboardIsOpened = false
    private var keyboardOpenJob: Job? = null

    private var keyboardStateWatcher: SoftKeyboardStateWatcher? = null
    private var keyboardStateListener =
        object : SoftKeyboardStateWatcher.SoftKeyboardStateListener {
            override fun onSoftKeyboardOpened(keyboardHeight: Int) {
                keyboardOpenJob?.cancel()
                keyboardOpenJob = lifecycleScope.launch {
                    if (!keyboardIsOpened) {
                        keyboardIsOpened = true
                        onKeyboardStateChanged(true)
                    }
                }
            }

            override fun onSoftKeyboardClosed() {
                keyboardOpenJob?.cancel()
                keyboardOpenJob = lifecycleScope.launch {
                    if (keyboardIsOpened) {
                        keyboardIsOpened = false
                        onKeyboardStateChanged(false)
                    }
                }
            }
        }

    abstract fun getViewBinding(): VB

    // lifecycle

    final override fun onAttach(context: Context) {
        super.onAttach(context)
        logDebug("onAttach", TAG)
    }

    final override fun onViewStateRestored(savedInstanceState: Bundle?) {
        super.onViewStateRestored(savedInstanceState)
        logDebug("onAttach", TAG)
    }

    final override fun onStart() {
        super.onStart()
        logDebug("onStart", TAG)
    }

    @CallSuper
    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?,
    ): View? {
        logDebug("onCreateView", TAG)
        viewBinding = getViewBinding()
        return viewBinding?.root
    }

    final override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        logDebug("onViewCreated", TAG)
        setupNavControllers()
        subscribeToBaseViewModel()
        super.onViewCreated(view, savedInstanceState)
        viewModel.onViewCreated()
        (activity as? MainActivity)?.alertDialogResultListener = this
        setupKeyboardStateListener()
        ViewCompat.setOnApplyWindowInsetsListener(view) { _, windowInsets ->
            val systemBarsInsets = windowInsets.getInsets(WindowInsetsCompat.Type.systemBars())
            val statusBarHeightFromInsets = systemBarsInsets.top

            keyboardStateWatcher?.setStatusBarOffset(statusBarHeightFromInsets)
            windowInsets
        }
        onCreated()
        setupView()
        setupControls()
        parseArguments()
        subscribeToLiveData()
    }

    private fun setupKeyboardStateListener() {
        keyboardStateWatcher = SoftKeyboardStateWatcher(requireActivity())
    }

    final override fun onResume() {
        logDebug("onResume", TAG)
        super.onResume()
        keyboardStateWatcher?.addSoftKeyboardStateListener(keyboardStateListener)
        if (this !is BaseFlowFragment) {
            val callback = object : OnBackPressedCallback(true) {
                override fun handleOnBackPressed() {
                    findParentFragmentByType(MainTabsFlowFragment::class.java)?.let { fragment ->
                        val lastVisibleFragment = fragment.childFragmentManager.fragments.lastOrNull { it.isVisible }

                        when (lastVisibleFragment) {
                            is NavHostFragment -> {
                                val visibleFragment = lastVisibleFragment.childFragmentManager.fragments.firstOrNull { it.isVisible }
                                when (visibleFragment) {
                                    is BaseFlowFragment<*, *> -> onBackPressed()
                                    is BaseFragment<*, *> -> visibleFragment.onBackPressed()
                                }
                            }
                            else -> onBackPressed()
                        }
                    } ?: run {
                        onBackPressed()
                    }
                }
            }
            requireActivity().onBackPressedDispatcher.addCallback(this, callback)
        }
        onResumed()
        viewModel.fragmentOnResume()
        viewModel.onResumed()
    }

    final override fun onPause() {
        logDebug("onPause", TAG)
        super.onPause()
        hideSettingsMenu()
        onPaused()
        viewModel.onPaused()
        keyboardStateWatcher?.removeSoftKeyboardStateListener(keyboardStateListener)
        onKeyboardStateChanged(false)
    }

    final override fun onStop() {
        logDebug("onStop", TAG)
        super.onStop()
        onStopped()
        viewModel.onStopped()
    }

    final override fun onDestroyView() {
        logDebug("onDestroyView", TAG)
        viewBinding = null
        listPopupWindow?.dismiss()
        popupWindow?.dismiss()
        viewModel.unbindFlowNavController()
        viewModel.unbindActivityNavController()
        if (viewModel.mainTabsEnum == null) {
            viewModel.unbindTabNavController()
        }
        super.onDestroyView()
    }

    final override fun onDestroy() {
        logDebug("onDestroy", TAG)
        super.onDestroy()
        onDestroyed()
        viewModel.onDestroyed()
    }

    final override fun onDetach() {
        logDebug("onDetach", TAG)
        super.onDetach()
        onDetached()
        viewModel.onDetached()
    }

    final override fun onAlertDialogResultReady(result: AlertDialogResult) {
        logDebug("onAlertDialogResult", TAG)
        onAlertDialogResult(result)
        viewModel.onAlertDialogResult()
        viewModel.onAlertDialogResult(result)
    }

    protected open fun onCreated() {
        // Override when needed
    }

    protected open fun setupView() {
        // Override when needed
    }

    protected open fun setupControls() {
        // Override when needed
    }

    protected open fun subscribeToLiveData() {
        // Override when needed
    }

    open fun onAlertDialogResult(result: AlertDialogResult) {
        // Override when needed
    }

    protected open fun onResumed() {
        // Override when needed
    }

    protected open fun parseArguments() {
        // Override when needed
    }

    protected open fun onPaused() {
        // Override when needed
    }

    protected open fun onStopped() {
        // Override when needed
    }

    protected open fun onDestroyed() {
        // Override when needed
    }

    protected open fun onDetached() {
        // Override when needed
    }

    protected open fun onKeyboardStateChanged(isOpened: Boolean) {
        logDebug("onKeyboardStateChanged isOpened: $isOpened", TAG)
        // Override when needed
    }

    protected open fun setupNavControllers() {
        setupActivityNavController()
        viewModel.bindFlowNavController(findNavController())
        findParentFragmentByType(MainTabsFlowFragment::class.java)?.let {
            it.navHostFragmentMap[viewModel.mainTabsEnum?.menuID]?.get()?.navController?.let { navController ->
                viewModel.bindTabNavController(navController)
            }
        }
    }

    protected fun setupActivityNavController() {
        viewModel.bindActivityNavController(findActivityNavController())
    }

    private fun subscribeToBaseViewModel() {
        viewModel.closeActivityLiveData.observe(viewLifecycleOwner) {
            activity?.finish()
        }
        viewModel.backPressedFailedLiveData.observe(viewLifecycleOwner) {
            try {
                ((parentFragment as? NavHostFragment)?.parentFragment as? BaseFlowFragment<*, *>)?.onExit()
            } catch (e: Exception) {
                logError("backPressedFailedLiveData Exception: ${e.message}", e, TAG)
            }
        }
        viewModel.showBannerMessageLiveData.observe(viewLifecycleOwner) {
            showMessage(it)
        }
        viewModel.showDialogMessageLiveData.observe(viewLifecycleOwner) {
            showMessage(it)
        }
        viewModel.uiState.observe(viewLifecycleOwner) {
            when (it) {
                is UiState.Ready -> showReadyState()
                is UiState.Empty -> showEmptyState()
            }
        }
        viewModel.showLoadingDialogLiveData.observe(viewLifecycleOwner) {
            when (it) {
                is FullscreenLoadingState.Loading -> (activity as? MessageBannerHolder)?.showFullscreenLoader(
                    it.message
                )

                is FullscreenLoadingState.Ready -> (activity as? MessageBannerHolder)?.hideFullscreenLoader()
            }
        }
        viewModel.loadingState.observe(viewLifecycleOwner) {
            when (it) {
                is LoadingState.Ready -> hideLoader()
                is LoadingState.Loading -> showLoader(
                    message = it.message,
                    translucent = it.translucent,
                )
            }
        }
        viewModel.errorState.observe(viewLifecycleOwner) {
            when (it) {
                is ErrorState.Ready -> hideErrorState()
                is ErrorState.Error -> showErrorState(
                    title = it.title,
                    iconRes = it.iconRes,
                    showIcon = it.showIcon,
                    showTitle = it.showTitle,
                    description = it.description,
                    showDescription = it.showDescription,
                    showActionOneButton = it.showActionTwoButton,
                    showActionTwoButton = it.showActionTwoButton,
                    actionOneButtonText = it.actionOneButtonText,
                    actionTwoButtonText = it.actionTwoButtonText,
                )
            }
        }
        viewModel.newFirebaseMessageLiveData.observe(viewLifecycleOwner) { message ->
            viewModel.onNewFirebaseMessage(message)
        }
    }

    final override fun showMessage(message: BannerMessage, anchorView: View?) {
        try {
            logDebug(
                "showMessage BannerMessage: ${message.message.getString(requireContext())}",
                TAG
            )
            (activity as? MessageBannerHolder)?.showMessage(message, anchorView)
        } catch (e: Exception) {
            logError("showMessage BannerMessage Exception: ${e.message}", e, TAG)
        }
    }

    final override fun showMessage(message: DialogMessage) {
        try {
            logDebug(
                "showMessage DialogMessage: ${message.message.getString(requireContext())}",
                TAG
            )
            (activity as? MessageBannerHolder)?.showMessage(message)
        } catch (e: Exception) {
            logError("showMessage DialogMessage Exception: ${e.message}", e, TAG)
        }
    }

    // hierarchy for view -> content, empty state, error state, loader

    fun showLoader(
        message: String? = null,
        translucent: Boolean = false,
    ) {
        try {
            val loaderView = view?.findViewById<FrameLayout>(R.id.loaderView)
            if (loaderView?.visibility != View.VISIBLE) {
                loaderView?.visibility = View.VISIBLE
            }
            if (!message.isNullOrEmpty() && loaderView?.visibility == View.VISIBLE) {
                logDebug("showLoader message: $message", TAG)
                val tvMessage = loaderView.findViewById<AppCompatTextView>(R.id.tvMessage)
                tvMessage?.text = message
            }
            val loaderLayout = view?.findViewById<FrameLayout>(R.id.loaderLayout)
            if (translucent) {
                loaderLayout?.setBackgroundColorResource(R.color.color_translucent)
            } else {
                loaderLayout?.setBackgroundColorResource(R.color.color_white)
            }
        } catch (e: Exception) {
            logError("showLoader Exception: ${e.message}", e, TAG)
        }
    }

    private fun hideLoader() {
        try {
            view?.findViewById<ComplexGestureRefreshView>(R.id.refreshLayout)?.isRefreshing = false
            val loaderView = view?.findViewById<FrameLayout>(R.id.loaderView)
            if (loaderView?.visibility != View.GONE) {
                loaderView?.visibility = View.GONE
            }
        } catch (e: Exception) {
            logError("hideLoader Exception: ${e.message}", e, TAG)
        }
    }

    override fun showFullscreenLoader(message: StringSource?) {
        (activity as MessageBannerHolder).showFullscreenLoader(message = message)
    }

    override fun hideFullscreenLoader() {
        (activity as MessageBannerHolder).hideFullscreenLoader()
    }

    protected fun showEmptyState() {
        logDebug("showEmptyState", TAG)
        try {
            val emptyStateView = view?.findViewById<FrameLayout>(R.id.emptyStateView)
            if (emptyStateView?.visibility != View.VISIBLE) {
                emptyStateView?.visibility = View.VISIBLE
            }
        } catch (e: Exception) {
            logError("showEmptyState Exception: ${e.message}", e, TAG)
        }
    }

    protected fun showReadyState() {
        logDebug("hideEmptyState", TAG)
        try {
            val emptyStateView = view?.findViewById<FrameLayout>(R.id.emptyStateView)
            if (emptyStateView?.visibility != View.GONE) {
                emptyStateView?.visibility = View.GONE
            }
        } catch (e: Exception) {
            logError("hideEmptyState Exception: ${e.message}", e, TAG)
        }
    }

    fun showErrorState(
        title: StringSource,
        description: StringSource,
        iconRes: Int? = null,
        showIcon: Boolean? = null,
        showTitle: Boolean? = null,
        showDescription: Boolean? = null,
        showActionOneButton: Boolean? = null,
        showActionTwoButton: Boolean? = null,
        actionOneButtonText: StringSource? = null,
        actionTwoButtonText: StringSource? = null,
    ) {
        logDebug("showErrorState", TAG)
        try {
            logDebug("showErrorState description: $description", TAG)
            val errorView = view?.findViewById<View>(R.id.errorView)
            val btnErrorActionOne = errorView?.findViewById<AppCompatButton>(R.id.btnErrorActionOne)
            val btnErrorActionTwo = errorView?.findViewById<AppCompatButton>(R.id.btnErrorActionTwo)
            val tvDescription =
                errorView?.findViewById<AppCompatTextView>(R.id.tvErrorViewDescription)
            val tvTitle =
                errorView?.findViewById<AppCompatTextView>(R.id.tvErrorViewTitle)
            val ivIcon =
                errorView?.findViewById<AppCompatImageView>(R.id.ivErrorIcon)
            errorView?.visibility = View.VISIBLE
            if (showTitle != null) {
                tvTitle?.isVisible = showTitle
            }
            if (showDescription != null) {
                tvDescription?.isVisible = showDescription
            }
            if (showActionOneButton != null) {
                btnErrorActionTwo?.isVisible = showActionOneButton
            }
            if (showActionTwoButton != null) {
                btnErrorActionTwo?.isVisible = showActionTwoButton
            }
            if (showIcon != null) {
                ivIcon?.isVisible = showIcon
            }
            tvTitle?.text = title.getString(requireContext())
            tvDescription?.text = description.getString(requireContext())
            if (actionOneButtonText != null) {
                btnErrorActionOne?.text = actionOneButtonText.getString(requireContext())
            }
            if (actionTwoButtonText != null) {
                btnErrorActionTwo?.text = actionTwoButtonText.getString(requireContext())
            }
            if (iconRes != null && iconRes != 0) {
                ivIcon?.setImageResource(iconRes)
            }
        } catch (e: Exception) {
            logError("showErrorState Exception: ${e.message}", e, TAG)
        }
    }

    private fun hideErrorState() {
        try {
            val errorView = view?.findViewById<FrameLayout>(R.id.errorView)
            errorView?.visibility = View.GONE
        } catch (e: Exception) {
            logError("hideErrorState Exception: ${e.message}", e, TAG)
        }
    }

    protected fun showSpinner(model: CommonSpinnerUi, anchor: View) {
        logDebug("showSpinner", TAG)
        hideKeyboard()
        if (model.list.isEmpty()) {
            showMessage(BannerMessage.error(StringSource("List it empty")))
            return
        }
        listPopupWindow = anchor.showSpinner(
            model = model,
            clickListener = {
                viewModel.onSpinnerSelected(
                    model.copy(
                        selectedValue = it
                    )
                )
            }
        )
    }

    private fun measureContentWidth(context: Context, adapter: ListAdapter): Int {
        val measureParentViewGroup = FrameLayout(context)
        var itemView: View? = null
        var maxWidth = 0
        var itemType = 0
        val widthMeasureSpec = View.MeasureSpec.makeMeasureSpec(0, View.MeasureSpec.UNSPECIFIED)
        val heightMeasureSpec = View.MeasureSpec.makeMeasureSpec(0, View.MeasureSpec.UNSPECIFIED)
        for (index in 0 until adapter.count) {
            val positionType = adapter.getItemViewType(index)
            if (positionType != itemType) {
                itemType = positionType
                itemView = null
            }
            itemView = adapter.getView(index, itemView, measureParentViewGroup)
            itemView.measure(widthMeasureSpec, heightMeasureSpec)
            val itemWidth = itemView.measuredWidth
            if (itemWidth > maxWidth) {
                maxWidth = itemWidth
            }
        }
        return maxWidth
    }

    protected fun showDatePicker(model: CommonDatePickerUi) {
        logDebug("showDatePicker", TAG)
        hideKeyboard()
        val now = System.currentTimeMillis()
        val selectedValue = when {
            model.selectedValue != null -> model.selectedValue
            model.minDate.timeInMillis > now -> model.minDate
            model.maxDate.timeInMillis < now -> model.maxDate
            else -> getCalendar()
        }
        val datePickerConfig = DatePickerConfig(
            minDate = model.minDate,
            maxDate = model.maxDate,
            selectedValue = selectedValue,
        )
        showDataPicker(
            datePickerConfig = datePickerConfig,
            dateSelected = {
                viewModel.onDatePickerChanged(
                    model = model.copy(
                        selectedValue = it,
                    ),
                )
            }
        )
    }

    protected fun setupDialogWithSearchResultListener() {
        logDebug("setupDialogWithSearchResultListener", TAG)
        findParentFragmentResultListenerFragmentManager()?.setFragmentResultListener(
            COMMON_BOTTOM_SHEET_WITH_SEARCH_FRAGMENT_RESULT_BUNDLE_KEY, viewLifecycleOwner
        ) { _, bundle ->
            bundle.getParcelableCompat<CommonDialogWithSearchUi>(
                COMMON_BOTTOM_SHEET_WITH_SEARCH_FRAGMENT_RESULT_DATA_KEY
            )?.let {
                viewModel.onDialogElementSelected(it)
            }
        }
    }

    protected fun setupDialogWithSearchMultiselectResultListener() {
        logDebug("setupDialogWithSearchMultiselectResultListener", TAG)
        findParentFragmentResultListenerFragmentManager()?.setFragmentResultListener(
            COMMON_BOTTOM_SHEET_WITH_SEARCH_MULTISELECT_FRAGMENT_RESULT_BUNDLE_KEY,
            viewLifecycleOwner
        ) { _, bundle ->
            bundle.getParcelableCompat<CommonDialogWithSearchMultiselectUi>(
                COMMON_BOTTOM_SHEET_WITH_SEARCH_MULTISELECT_FRAGMENT_RESULT_DATA_KEY
            )?.let {
                viewModel.onDialogMultiselectSelected(it)
            }
        }
    }

    open fun onBackPressed() {
        // Default on back pressed implementation for fragments.
        logDebug("onBackPressed", TAG)
        viewModel.onBackPressed()
    }

    private fun hideSettingsMenu() {
        popupWindow?.setOnDismissListener {
            popupWindow = null
        }
        popupWindow?.dismiss()
    }


}
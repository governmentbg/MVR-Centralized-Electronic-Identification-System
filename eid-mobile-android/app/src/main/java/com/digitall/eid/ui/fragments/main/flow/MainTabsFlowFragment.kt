/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.main.flow

import android.Manifest
import android.os.Build
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.activity.result.contract.ActivityResultContracts
import androidx.core.view.WindowInsetsCompat
import androidx.core.view.isVisible
import androidx.navigation.fragment.NavHostFragment
import androidx.navigation.fragment.navArgs
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentMainTabsFlowContainerBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.models.common.BannerMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.ui.fragments.base.flow.BaseFlowFragment
import org.koin.androidx.viewmodel.ext.android.viewModel
import java.lang.ref.WeakReference

class MainTabsFlowFragment :
    BaseFlowFragment<FragmentMainTabsFlowContainerBinding, MainTabsFlowViewModel>() {

    companion object {
        private const val TAG = "MainTabsFlowFragmentTag"
    }

    override fun getViewBinding() = FragmentMainTabsFlowContainerBinding.inflate(layoutInflater)

    private val args: MainTabsFlowFragmentArgs by navArgs()

    override val viewModel: MainTabsFlowViewModel by viewModel()

    val navHostFragmentMap = mutableMapOf<Int, WeakReference<NavHostFragment>>()

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?
    ): View? {
        activity?.window?.decorView?.setOnApplyWindowInsetsListener { view, insets ->
            val insetsCompat = WindowInsetsCompat.toWindowInsetsCompat(insets)
            val isImeVisible = insetsCompat.isVisible(WindowInsetsCompat.Type.ime())

            if (isVisible) {
                binding.bottomNavigationView.isVisible = !isImeVisible
            }

            view.onApplyWindowInsets(insets)
        }
        return super.onCreateView(inflater, container, savedInstanceState)
    }

    override fun setupControls() {
        logDebug("setupControls", TAG)
        if (navHostFragmentMap.values.isEmpty()) {
            setupBottomNavigation()
        }
        binding.bottomNavigationView.selectedItemId = args.tabId
    }

    private fun setupBottomNavigation() {
        logDebug("setupBottomNavigation", TAG)
        try {
            binding.bottomNavigationView.setOnItemSelectedListener { item ->
                val selectedGraphId = MainTabsEnum.findNavigationIDByMenuID(item.itemId)
                return@setOnItemSelectedListener if (selectedGraphId != null) {
                    if (!navHostFragmentMap.containsKey(item.itemId)) {
                        logDebug("navHostFragmentMap add fragment", TAG)
                        val navHostFragment = NavHostFragment.create(selectedGraphId)
                        navHostFragmentMap[item.itemId] = WeakReference(navHostFragment)
                        childFragmentManager.beginTransaction()
                            .add(
                                R.id.flowTabsNavigationContainer,
                                navHostFragment,
                                item.itemId.toString()
                            )
                            .commitNow()
                    }
                    navHostFragmentMap.forEach { (itemId, weakRefNavHostFragment) ->
                        val fragment = weakRefNavHostFragment.get()
                        if (fragment != null) {
                            childFragmentManager.beginTransaction().apply {
                                if (itemId == item.itemId) {
                                    show(fragment)
                                } else {
                                    hide(fragment)
                                }
                                commit()
                            }
                        }
                    }
                    true
                } else false
            }
        } catch (e: Exception) {
            logError("parseArguments Exception: ${e.message}", e, TAG)
            showMessage(BannerMessage.error(StringSource(R.string.error_internal_error_short)))
        }
    }

    override fun onCreated() {
        super.onCreated()

        if (Build.VERSION.SDK_INT > Build.VERSION_CODES.S_V2) {
            val requestPermission =
                registerForActivityResult(ActivityResultContracts.RequestPermission()) {}
            requestPermission.launch(Manifest.permission.POST_NOTIFICATIONS)
        }
    }
}
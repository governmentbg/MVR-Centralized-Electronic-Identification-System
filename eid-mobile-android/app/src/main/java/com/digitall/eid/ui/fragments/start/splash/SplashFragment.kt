/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.start.splash

import com.digitall.eid.databinding.FragmentSplashBinding
import com.digitall.eid.ui.fragments.base.BaseFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class SplashFragment :
    BaseFragment<FragmentSplashBinding, SplashViewModel>() {

    companion object {
        private const val TAG = "SplashFragmentTag"
    }

    override fun getViewBinding() = FragmentSplashBinding.inflate(layoutInflater)

    override val viewModel: SplashViewModel by viewModel()

}
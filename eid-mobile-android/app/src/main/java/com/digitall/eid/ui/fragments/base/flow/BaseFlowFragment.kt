/**
 * This fragment is used for navigation so that you can go to the flow, and not individual fragments
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 */
package com.digitall.eid.ui.fragments.base.flow

import androidx.annotation.NavigationRes
import androidx.navigation.fragment.NavHostFragment
import androidx.viewbinding.ViewBinding
import com.digitall.eid.R
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StartDestination
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.BaseViewModel
import com.digitall.eid.ui.fragments.base.BaseFragment
import kotlin.system.exitProcess

abstract class BaseFlowFragment<VB : ViewBinding, VM : BaseViewModel> : BaseFragment<VB, VM>() {

    companion object {
        private const val TAG = "BaseFlowFragmentTag"
    }

    /**
     * When your flow has several children, you should override this
     * method and provide the flow graph.
     */
    @NavigationRes
    protected open fun getFlowGraph(): Int? = null

    /**
     * Use this method to specify a start dynamic destination for
     * the flow graph in [getFlowGraph].
     */
    protected open fun getStartDestination(): StartDestination? = null

    override fun setupNavControllers() {
        logDebug("setupNavControllers", TAG)
        setupActivityNavController()
        val flowGraph = getFlowGraph()
        if (flowGraph != null) {
            // Search for the flow controller
            val host = childFragmentManager
                .findFragmentById(R.id.flowNavigationContainer) as NavHostFragment
            try {
                // Try to get the current graph, if it is there, nav controller is valid.
                // When there is no graph, it throws IllegalStateException,
                // then we need to create a graph ourselves
                host.navController.graph
            } catch (e: Exception) {
                val graphInflater = host.navController.navInflater
                val graph = graphInflater.inflate(flowGraph)
                val startDestination = getStartDestination()?.also {
                    graph.setStartDestination(it.destination)
                }
                host.navController.setGraph(graph, startDestination?.arguments)
            }
            viewModel.bindFlowNavController(host.navController)
        }
    }

    fun onExit() {
        logDebug("onExit", TAG)
        showMessage(
            DialogMessage(
                messageID = DIALOG_EXIT,
                message = StringSource(R.string.exit_application_message),
                title = StringSource(R.string.information),
                positiveButtonText = StringSource(R.string.yes),
                negativeButtonText = StringSource(R.string.no),
            )
        )
    }

    override fun onAlertDialogResult(result: AlertDialogResult) {
        if (result.messageId == DIALOG_EXIT && result.isPositive) {
            exitProcess(0)
        }
    }
}
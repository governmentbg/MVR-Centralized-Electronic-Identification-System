/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.scanner.scanner

import android.Manifest
import android.content.Intent
import android.content.pm.PackageManager
import android.net.Uri
import android.provider.Settings
import android.view.View
import androidx.activity.result.contract.ActivityResultContracts
import androidx.annotation.OptIn
import androidx.camera.core.CameraSelector
import androidx.camera.core.ExperimentalGetImage
import androidx.camera.core.Preview
import androidx.camera.lifecycle.ProcessCameraProvider
import androidx.core.content.ContextCompat
import androidx.core.view.isVisible
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentScanCodeBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.models.common.AlertDialogResult
import com.digitall.eid.models.common.BannerMessage
import com.digitall.eid.models.common.DialogMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.google.mlkit.vision.barcode.BarcodeScanning
import com.google.mlkit.vision.common.InputImage
import org.koin.androidx.viewmodel.ext.android.viewModel

class ScanCodeFragment : BaseFragment<FragmentScanCodeBinding, ScanCodeViewModel>() {

    companion object {
        private const val TAG = "ScanCodeFragmentTag"
        private const val DIALOG_PERMISSIONS = "DIALOG_PERMISSIONS"
        private const val DIALOG_OPEN = "DIALOG_OPEN"
        private const val DIALOG_ERROR = "DIALOG_ERROR"
    }

    override fun getViewBinding() = FragmentScanCodeBinding.inflate(layoutInflater)

    override val viewModel: ScanCodeViewModel by viewModel()

    private val requestPermissionLauncher =
        registerForActivityResult(ActivityResultContracts.RequestPermission()) { isGranted: Boolean ->
            if (!isGranted) {
                showPermissionExplanation()
            }
        }

    override fun onCreated() {
        when {
            shouldShowRequestPermissionRationale(Manifest.permission.CAMERA) -> {
                showPermissionExplanation()
            }

            else -> {
                requestPermissionLauncher.launch(Manifest.permission.CAMERA)
            }
        }
    }

    override fun setupControls() {
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
        binding.errorView.actionOneClickListener = {
            viewModel.refreshScreen()
        }
        binding.errorView.actionTwoClickListener = {
            viewModel.refreshScreen()
        }
        binding.emptyStateView.reloadClickListener = {
            viewModel.refreshScreen()
        }
        binding.btnStartScan.onClickThrottle {
            startScan()
        }
    }

    override fun subscribeToLiveData() {
        viewModel.dialogMessageLiveData.observe(viewLifecycleOwner) {
            showMessage(
                DialogMessage(
                    message = it,
                    messageID = DIALOG_OPEN,
                    title = StringSource(R.string.information),
                    positiveButtonText = StringSource(R.string.yes),
                    negativeButtonText = StringSource(R.string.no),
                )
            )
        }
        viewModel.dialogError.observe(viewLifecycleOwner) {
            showMessage(
                DialogMessage(
                    message = it,
                    messageID = DIALOG_ERROR,
                    title = StringSource(R.string.information),
                    positiveButtonText = StringSource(R.string.ok),
                )
            )
        }
    }

    private fun startScan() {
        when {
            ContextCompat.checkSelfPermission(
                requireContext(),
                Manifest.permission.CAMERA
            ) == PackageManager.PERMISSION_GRANTED -> {
                startCamera()
            }

            shouldShowRequestPermissionRationale(Manifest.permission.CAMERA) -> {
                showPermissionExplanation()
            }

            else -> {
                requestPermissionLauncher.launch(Manifest.permission.CAMERA)
            }
        }
    }


    @OptIn(ExperimentalGetImage::class)
    private fun startCamera() {
        logDebug("startCamera", TAG)
        val cameraProviderFuture = ProcessCameraProvider.getInstance(requireContext())
        cameraProviderFuture.addListener({
            val cameraProvider: ProcessCameraProvider = cameraProviderFuture.get()
            val preview = Preview.Builder().build().also {
                it.surfaceProvider = binding.previewView.surfaceProvider
            }
            val cameraSelector = CameraSelector.DEFAULT_BACK_CAMERA
            val analysisUseCase = androidx.camera.core.ImageAnalysis.Builder()
                .build()
                .also {
                    it.setAnalyzer(ContextCompat.getMainExecutor(requireContext())) { imageProxy ->
                        val mediaImage = imageProxy.image
                        if (mediaImage != null) {
                            val image = InputImage.fromMediaImage(
                                mediaImage,
                                imageProxy.imageInfo.rotationDegrees
                            )
                            BarcodeScanning.getClient().process(image)
                                .addOnSuccessListener { barcodes ->
                                    for (barcode in barcodes) {
                                        val rawValue = barcode.rawValue
                                        logDebug("rawValue: $rawValue", TAG)
                                        if (!rawValue.isNullOrEmpty()) {
                                            viewModel.onScanCodeSuccess(rawValue)
                                        }
                                    }
                                }
                                .addOnCompleteListener {
                                    imageProxy.close()
                                }
                        } else {
                            showMessage(BannerMessage.error(StringSource("Error start camera")))
                        }
                    }
                }
            try {
                cameraProvider.unbindAll()
                cameraProvider.bindToLifecycle(this, cameraSelector, preview, analysisUseCase)
                binding.btnStartScan.visibility = View.INVISIBLE
                binding.icScanPreview.isVisible = false
                binding.previewView.isVisible = true
            } catch (e: Exception) {
                logError("startCamera Exception: ${e.message}", e, TAG)
                showMessage(BannerMessage.error(StringSource("Error start camera")))
            }
        }, ContextCompat.getMainExecutor(requireContext()))
    }

    private fun showPermissionExplanation() {
        showMessage(
            DialogMessage(
                messageID = DIALOG_PERMISSIONS,
                message = StringSource("No camera permissions, please allow the use of the camera.\nOpen application settings?"),
                title = StringSource(R.string.information),
                positiveButtonText = StringSource(R.string.yes),
                negativeButtonText = StringSource(R.string.no),
            )
        )
    }

    override fun onAlertDialogResult(result: AlertDialogResult) {
        when (result.messageId) {
            DIALOG_PERMISSIONS -> {
                if (result.isPositive) {
                    try {
                        val intent = Intent(Settings.ACTION_APPLICATION_DETAILS_SETTINGS).apply {
                            data = Uri.fromParts("package", requireContext().packageName, null)
                        }
                        startActivity(intent)
                    } catch (e: Exception) {
                        logError("startCamera Exception: ${e.message}", e, TAG)
                        showMessage(BannerMessage.error(StringSource("Error start camera")))
                    }
                } else {
                    onBackPressed()
                }
            }

            DIALOG_OPEN -> {}
        }
    }

    override fun onDestroyed() {
        ProcessCameraProvider.getInstance(requireContext()).get().unbindAll()
    }

}
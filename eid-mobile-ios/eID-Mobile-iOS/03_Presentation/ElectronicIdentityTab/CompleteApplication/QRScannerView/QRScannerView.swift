//
//  QRScanView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 24.04.24.
//

import SwiftUI
import AVKit


struct QRScanerView: View {
    // MARK: - Properties
    @StateObject private var qrDelegate = QRScannerDelegate()
    @State private var session = AVCaptureSession()
    @State private var cameraPermission: Permission = .idle
    @State private var isScanning: Bool = false
    @State private var qrOutput = AVCaptureMetadataOutput()
    @State private var scannedCode: String = ""
    var onCodeScanned: ((String) -> Void)
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: 8) {
            Spacer()
            Text ("scan_qr_code_title".localized())
                .font(.heading3)
                .lineSpacing(4)
                .foregroundStyle(Color.textWhite)
            Spacer()
            VStack(spacing: 32) {
                //                Text ("ready_to_scan_title".localized())
                //                    .font(.heading4)
                //                    .lineSpacing(8)
                //                    .foregroundStyle(Color.textDefault)
                //                Spacer()
                GeometryReader {
                    let size = $0.size
                    ZStack {
                        CameraView(frameSize: CGSize(width: size.width,
                                                     height: size.width),
                                   session: $session)
                        .scaleEffect(0.97)
                        ForEach(0...3, id: \.self) { index in
                            let rotation = Double(index) * 90
                            RoundedRectangle(cornerRadius: 2, style: .circular)
                            /// Trimming to get Scanner like Edges
                                .trim(from: 0.61, to: 0.64)
                                .stroke(Color.buttonConfirm,
                                        style: StrokeStyle(lineWidth: 2,
                                                           lineCap: .square,
                                                           lineJoin: .miter))
                                .rotationEffect(.init(degrees: rotation))
                        }
                    }
                    /// Sqaure Shape
                    .frame(width: size.width, height: size.width)
                    /// Scanner Animation
                    .overlay(alignment: .top, content: {
                        Rectangle()
                            .fill(Color.buttonConfirm)
                            .frame(height: 2)
                            .shadow(color: .black.opacity(0.8), radius: 8, x: 0, y: 15)
                            .offset(y: isScanning ? size.width : 0)
                    })
                    /// To Make it Center
                    .frame(maxWidth: .infinity, maxHeight: .infinity)
                }
                .padding(.horizontal, 45)
                Spacer()
                Button {
                    if !session.isRunning && cameraPermission == .approved {
                        activateCamera()
                    }
                } label: {
                    Image (systemName: "qrcode.viewfinder")
                        .font(.largeTitle)
                        .foregroundColor(.gray)
                }
            }
            .padding(16)
            .background(RoundedRectangle(cornerRadius: 8).fill(Color.backgroundWhite))
            Spacer()
        }
        .padding()
        .background(Color.themeSecondaryDark)
        .onAppear {
            checkCameraPermission()
        }
        .onChange(of: qrDelegate.scannedCode) { newValue in
            if let code = newValue {
                scannedCode = code
                deactivateCamera()
                onCodeScanned(code)
            }
        }
    }
    
    /// Activate/Deactivate scanning
    private func activateCamera() {
        DispatchQueue.global(qos: .background).async {
            session.startRunning()
        }
        scannedCode = ""
        activateScannerAnimation()
    }
    
    private func deactivateCamera() {
        session.stopRunning()
        deactivateScannerAnimation()
    }
    
    /// Scanning animation
    private func activateScannerAnimation() {
        withAnimation(.easeInOut(duration: 0.85).delay(0.1).repeatForever(autoreverses: true)) {
            isScanning = true
        }
    }
    
    private func deactivateScannerAnimation() {
        withAnimation(.easeInOut(duration: 0.85)) {
            isScanning = false
        }
    }
    
    /// Checking camera permission
    private func checkCameraPermission() {
        Task {
            switch AVCaptureDevice.authorizationStatus(for: .video) {
            case .authorized:
                cameraPermission = .approved
                setupCamera()
            case .notDetermined:
                if await AVCaptureDevice.requestAccess(for: .video) {
                    cameraPermission = .approved
                    setupCamera()
                } else {
                    cameraPermission = .denied
                }
            case .denied,
                    .restricted:
                cameraPermission = .denied
            default: break
            }
        }
    }
    
    private func setupCamera() {
        do {
            /// Finding Back Camera
            guard let device = AVCaptureDevice.DiscoverySession(deviceTypes: [.builtInWideAngleCamera],
                                                                mediaType: .video,
                                                                position: .back).devices.first
            else {
                debugPrint("ERROR: Unable to connect to camera")
                return
            }
            /// Camera Input
            let input = try AVCaptureDeviceInput(device: device)
            /// Checking Whether input & output can be added to the session
            guard session.canAddInput(input),
                  session.canAddOutput(qrOutput) else {
                debugPrint("ERROR: Camera input/output error")
                return
            }
            session.beginConfiguration()
            session.addInput(input)
            session.addOutput(qrOutput)
            qrOutput.metadataObjectTypes = [.qr]
            qrOutput.setMetadataObjectsDelegate(qrDelegate, queue: .main)
            session.commitConfiguration()
            /// Note Session must be started on Background thread
            activateCamera()
        } catch {
            print(error.localizedDescription)
        }
    }
}


// MARK: - Preview
#Preview {
    QRScanerView(onCodeScanned: { _ in })
}

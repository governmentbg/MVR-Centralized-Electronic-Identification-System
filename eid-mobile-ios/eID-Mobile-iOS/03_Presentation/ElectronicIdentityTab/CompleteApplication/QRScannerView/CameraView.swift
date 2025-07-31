//
//  CameraView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 29.04.24.
//

import SwiftUI
import AVKit


struct CameraView: UIViewRepresentable {
    // MARK: - Properties
    var frameSize: CGSize
    @Binding var session: AVCaptureSession
    
    // MARK: - UI
    func makeUIView(context: Context) -> UIView {
        let view = UIViewType(frame: CGRect(origin: .zero, size: frameSize))
        view.backgroundColor = .clear
        let cameraLayer = AVCaptureVideoPreviewLayer(session: session)
        cameraLayer.frame = .init(origin: .zero, size: frameSize)
        cameraLayer.videoGravity = .resizeAspectFill
        cameraLayer.masksToBounds = true
        view.layer.addSublayer(cameraLayer)
        
        return view
    }
    
    func updateUIView(_ uiView: UIViewType, context: Context) {
        
    }
}

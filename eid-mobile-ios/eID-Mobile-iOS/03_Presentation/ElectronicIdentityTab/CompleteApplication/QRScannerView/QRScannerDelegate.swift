//
//  QRScannerDelegate.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 29.04.24.
//

import Foundation
import AVKit


class QRScannerDelegate: NSObject, ObservableObject, AVCaptureMetadataOutputObjectsDelegate {
    // MARK: - Properties
    @Published var scannedCode: String?
    
    // MARK: - Methods
    func metadataOutput(_ output: AVCaptureMetadataOutput, didOutput metadataObjects: [AVMetadataObject], from connection: AVCaptureConnection) {
        guard let metaObject = metadataObjects.first,
              let readableObject = metaObject as? AVMetadataMachineReadableCodeObject,
              let code = readableObject.stringValue
        else {
            return
        }
        scannedCode = code 
    }
}

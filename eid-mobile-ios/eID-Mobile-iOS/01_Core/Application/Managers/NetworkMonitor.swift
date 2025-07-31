//
//  NetworkMonitor.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 7.02.24.
//

import Foundation
import Alamofire


class NetworkMonitor: ObservableObject {
    // MARK: - Properties
    static let shared = NetworkMonitor()
    @Published var isConnected: Bool = true
    private let reachabilityManager = Alamofire.NetworkReachabilityManager()
    
    // MARK: - Init
    private init() {
        reachabilityManager?.startListening { status in
            switch status {
            case .notReachable,
                    .unknown:
                self.isConnected = false
            case .reachable(.ethernetOrWiFi),
                    .reachable(.cellular):
                self.isConnected = true
            }
        }
    }
}

//
//  APICallerViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 17.10.23.
//

import SwiftUI

/** Protocol with default properties for ViewModels with API calls */
protocol APICallerViewModel where Self: ObservableObject {
    // MARK: - Properties
    var showLoading: Bool { get set }
    var showError: Bool { get set }
    var errorText: String { get set }
}

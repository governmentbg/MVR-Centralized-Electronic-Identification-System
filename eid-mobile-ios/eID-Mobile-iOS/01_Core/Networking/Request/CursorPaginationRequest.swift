//
//  CursorPaginationRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 23.01.24.
//

import Foundation


/** Protocol with default parameters required for requests with cursor pagination */
protocol CursorPaginationRequest where Self: Codable {
    // MARK: - Properties
    var cursorSize: Int { get }
    var cursorSearchAfter: [String] { get }
}

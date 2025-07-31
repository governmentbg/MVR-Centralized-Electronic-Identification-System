//
//  CursorPaginationResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 23.01.24.
//

import Foundation


/** Protocol with default parameters required for requests with pagination */
protocol CursorPaginationResponse where Self: Codable {
    // MARK: - Properties
    var searchAfter: [Int] { get }
}

//
//  EdgeBorder.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 25.10.23.
//

import SwiftUI


struct EdgeBorder: Shape {
    // MARK: - Properties
    var width: CGFloat
    var edges: [Edge]
    
    // MARK: - Path generation
    func path(in rect: CGRect) -> Path {
        edges.map { edge -> Path in
            switch edge {
            case .top:
                return Path(.init(x: rect.minX,
                                  y: rect.minY,
                                  width: rect.width,
                                  height: width))
            case .bottom:
                return Path(.init(x: rect.minX,
                                  y: rect.maxY - width,
                                  width: rect.width,
                                  height: width))
            case .leading:
                return Path(.init(x: rect.minX,
                                  y: rect.minY,
                                  width: width,
                                  height: rect.height))
            case .trailing:
                return Path(.init(x: rect.maxX - width,
                                  y: rect.minY,
                                  width: width,
                                  height: rect.height))
            }
        }.reduce(into: Path()) { $0.addPath($1) }
    }
}

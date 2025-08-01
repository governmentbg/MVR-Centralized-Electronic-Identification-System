//
//  EIDSegmentedPicker.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 20.10.23.
//

import Foundation
import SwiftUI


public struct EIDSegmentedPicker<Element, Content>: View where Content: View {
    // MARK: - Types
    public typealias Elements = [Element]
    
    // MARK: - Properties
    private let items: Elements
    private let content: (Elements.Element, Bool) -> Content
    @Binding private var selection: Elements.Index
    
    // MARK: - Init
    public init(_ items: Elements, selection: Binding<Elements.Index>, @ViewBuilder content: @escaping (Elements.Element, Bool) -> Content) {
        self.items = items
        self._selection = selection
        self.content = content
    }
    
    // MARK: - Body
    public var body: some View {
        HStack(spacing: 0) {
            ForEach(items.indices, id: \.self) { index in
                Button(action: {
                    selection = index
                }, label: {
                    content(items[index], selection == index)
                })
            }
        }
    }
}

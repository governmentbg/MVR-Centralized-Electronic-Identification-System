//
//  Prefix+Extension.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 15.05.24.
//

import SwiftUI


prefix func ! (value: Binding<Bool>) -> Binding<Bool> {
    Binding<Bool>(
        get: { !value.wrappedValue },
        set: { value.wrappedValue = !$0 }
    )
}

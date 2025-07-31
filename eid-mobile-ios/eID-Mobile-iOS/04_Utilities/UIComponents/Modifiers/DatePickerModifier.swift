//
//  DatePickerModifier.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 3.11.23.
//

import SwiftUI


struct DatePickerModifier: ViewModifier {
    // MARK: - Properties
    @Binding var showPicker: Bool
    @Binding var selectedDate: Date
    var range: ClosedRange<Date>? = nil
    var rangeFrom: PartialRangeFrom<Date>? = nil
    var rangeThrough: PartialRangeThrough<Date>? = nil
    var title: String
    var submitButtonTitle: String
    var onSubmit: (() -> Void)? = nil
    var canClear: Bool = false
    var clearButtonTitle: String = ""
    var onClear: (() -> Void)? = nil
    
    // MARK: - Body
    func body(content: Content) -> some View {
        content
            .sheet(isPresented: $showPicker) {
                VStack {
                    if let range = range {
                        DatePicker(title,
                                   selection: $selectedDate,
                                   in: range,
                                   displayedComponents: [.date])
                        .datePickerStyle(.graphical)
                        .labelsHidden()
                        .preferredColorScheme(.light)
                        .tint(Color.textActive)
                        .background(Color.backgroundWhite)
                        .presentationDetents([.medium])
                        .presentationCompactAdaptation(.sheet)
                    } else if let range = rangeFrom {
                        DatePicker(title,
                                   selection: $selectedDate,
                                   in: range,
                                   displayedComponents: [.date])
                        .datePickerStyle(.graphical)
                        .labelsHidden()
                        .preferredColorScheme(.light)
                        .tint(Color.textActive)
                        .background(Color.backgroundWhite)
                        .presentationDetents([.medium])
                        .presentationCompactAdaptation(.sheet)
                    } else if let range = rangeThrough {
                        DatePicker(title,
                                   selection: $selectedDate,
                                   in: range,
                                   displayedComponents: [.date])
                        .datePickerStyle(.graphical)
                        .labelsHidden()
                        .preferredColorScheme(.light)
                        .tint(Color.textActive)
                        .background(Color.backgroundWhite)
                        .presentationDetents([.medium])
                        .presentationCompactAdaptation(.sheet)
                    } else {
                        DatePicker(title,
                                   selection: $selectedDate,
                                   displayedComponents: [.date])
                        .datePickerStyle(.graphical)
                        .labelsHidden()
                        .preferredColorScheme(.light)
                        .tint(Color.textActive)
                        .background(Color.backgroundWhite)
                        .presentationDetents([.medium])
                        .presentationCompactAdaptation(.sheet)
                    }
                    HStack {
                        if canClear {
                            Button(action: {
                                showPicker.toggle()
                                onClear?()
                            }, label: {
                                Text(clearButtonTitle)
                            })
                            .buttonStyle(EIDButton(buttonState: .danger))
                        }
                        Button(action: {
                            showPicker.toggle()
                            onSubmit?()
                        }, label: {
                            Text(submitButtonTitle)
                        })
                        .buttonStyle(EIDButton())
                    }
                }
                .padding()
            }
    }
}

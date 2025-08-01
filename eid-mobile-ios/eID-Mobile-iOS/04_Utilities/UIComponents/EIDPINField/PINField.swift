//
//  PINField.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 21.05.24.
//

import SwiftUI


struct PINField: View {
    // MARK: - Properties
    var digitsCount: Int
    var type: PINFieldType 
    @Binding var pin: String
    @State var showPin = false
    @State var isDisabled = false
    @FocusState private var isFocused: Bool
    
    // MARK: - Body
    public var body: some View {
        HStack {
            Spacer()
            VStack(spacing: 18) {
                ZStack {
                    hiddenField
                    pinSlots
                }
                showPinView
            }
            Spacer()
        }
        .frame(maxWidth: .infinity)
        .padding()
        .onTapGesture {
            if !isDisabled {
                isFocused = true
            }
        }
        .onAppear {
            if !isDisabled && type == .pin {
                isFocused = true
            }
        }
    }
    
    // MARK: - Child views
    private var pinSlots: some View {
        HStack(alignment: .bottom) {
            ForEach(0..<digitsCount, id: \.self) { index in
                VStack(spacing: 0) {
                    Text(getPinDigitForIndex(index))
                        .font(.hero)
                        .lineSpacing(8)
                        .foregroundStyle(isDisabled ? Color.textInactive : Color.textDefault)
                    Rectangle()
                        .fill(((pin.count == index && isFocused) || getPinDigitForIndex(index) != " ") ? Color.textActive : Color.themePrimaryLight)
                        .frame(height: 3)
                        .frame(maxWidth: 58)
                }
            }
        }
    }
    
    private var hiddenField: some View {
        TextField("", text: $pin)
            .keyboardType(.numberPad)
            .limitLength(value: $pin,
                         length: digitsCount)
            .focused($isFocused)
            .frame(width: 0, height: 0)
            .toolbar {
                ToolbarItemGroup(placement: .keyboard) {
                    if isFocused {
                        Spacer()
                        Button("btn_done".localized()) {
                            isFocused = false
                        }
                        .buttonStyle(EIDButton(size: .small))
                    } else {
                        EmptyView()
                    }
                }
            }
            .disabled(isDisabled)
    }
    
    private var showPinView: some View {
        HStack {
            Spacer()
            Button(action: {
                showPin.toggle()
            }, label: {
                Image(systemName: showPin ? "eye.slash.fill" : "eye.fill")
                    .foregroundColor(Color.textActive)
            })
            .opacity(pin.isEmpty ? 0 : 1)
        }
        .frame(minHeight: 18)
    }
    
    // MARK: - Helpers
    private func getPinDigitForIndex(_ index: Int) -> String {
        let digits = pin.digits
        guard !digits.isEmpty,
              index < digits.count
        else { return " " }
        return showPin ? String(digits[index]) : "*"
    }
}

extension PINField {
    enum PINFieldType {
        case pin, can
    }
}


// MARK: - Preview
#Preview {
    PINField(digitsCount: 6,
             type: .pin,
             pin: .constant("123456"))
}

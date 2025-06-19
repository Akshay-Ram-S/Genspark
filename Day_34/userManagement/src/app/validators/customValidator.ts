import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function bannedWordsValidator(bannedWords: string[]): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value?.toLowerCase().trim() || '';

    const isBanned = bannedWords.map(w => w.toLowerCase()).includes(value);

    return isBanned ? { bannedWord: true } : null;
  };
}


export function matchPasswords(pass: string, confirmPass: string): ValidatorFn {
    const validator: ValidatorFn = (group: AbstractControl): ValidationErrors | null => {
    const password = group.get(pass)?.value;
    const confirmPassword = group.get(confirmPass)?.value;
    let result: ValidationErrors | null;

    if (password === confirmPassword) {
      result = null;
    } else {
      result = { passwordMismatch: true };
    }

    return result;
    };

    return validator;
}


export function validatePassword(): ValidatorFn {
  function validate(control: AbstractControl): ValidationErrors | null {
    const value = control.value || '';
    const hasMinLength = value.length >= 6;
    const hasNumber = /\d/.test(value);
    const hasSymbol = /[^A-Za-z0-9]/.test(value);
    let result: ValidationErrors | null;

    if (hasMinLength && hasNumber && hasSymbol) {
      result = null;
    } else {
      result = {
        passwordRequirements: {
          minLength: hasMinLength,
          hasNumber: hasNumber,
          hasSymbol: hasSymbol
        }
      };
    }

    return result;
  }

  return validate;
}


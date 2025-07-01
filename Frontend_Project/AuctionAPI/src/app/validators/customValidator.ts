import { AbstractControl, ValidationErrors } from '@angular/forms';

export function panValidator(control: AbstractControl): ValidationErrors | null {
  const value = control.value;
  if (!value) return null;

  const panRegex = /^[A-Z]{5}[0-9]{4}[A-Z]{1}$/;

  if (!panRegex.test(value)) {
    if (value.length !== 10) 
      return { panLength: true };
    if (!/^[A-Z]{5}/.test(value)) 
      return { panStart: true };
    if (!/[0-9]{4}/.test(value.slice(5, 9))) 
      return { panDigits: true };
    if (!/[A-Z]$/.test(value)) 
      return { panEnd: true };
    
    return { panInvalid: true };
  }

  return null;
}

export function aadharValidator(control: AbstractControl): ValidationErrors | null {
  const value = control.value;
  if (!value) 
    return null;

  if (!/^\d+$/.test(value)) 
    return { aadharNotDigits: true };
  if (value.length !== 12) 
    return { aadharLength: true };

  return null;
}

export function matchPasswords(passwordKey: string, confirmPasswordKey: string) {
  return (group: AbstractControl): ValidationErrors | null => {
    const password = group.get(passwordKey)?.value;
    const confirmPassword = group.get(confirmPasswordKey)?.value;

    if (password !== confirmPassword) {
      group.get(confirmPasswordKey)?.setErrors({ passwordMismatch: true });
    } 
    else {
      group.get(confirmPasswordKey)?.setErrors(null);
    }
    return null;
  };

}


export function futureDateValidator(control: AbstractControl): ValidationErrors | null {
  const selectedDate = new Date(control.value);
  const now = new Date();

  if (isNaN(selectedDate.getTime())) {
    return null;
  }

  return selectedDate > now ? null : { pastDate: 'End date must be in the future' };
}


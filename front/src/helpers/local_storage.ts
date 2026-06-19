// Auth tokens are stored in sessionStorage (not localStorage) so that each
// browser tab keeps its own independent session. This lets you be logged in
// as different accounts in different tabs at the same time.

function SaveValueByKey(key: string, value: string): boolean {
  try {
    sessionStorage.setItem(key, value);
    return true;
  } catch (error) {
    console.error(`Error saving to sessionStorage for key '${key}':`, error);
    return false;
  }
}

function ReadValueByKey(key: string): string | null {
  try {
    return sessionStorage.getItem(key);
  } catch (error) {
    console.error(`Error reading from sessionStorage for key '${key}':`, error);
    return null;
  }
}

function RemoveValueByKey(key: string): boolean {
  try {
    sessionStorage.removeItem(key);
    return true;
  } catch (error) {
    console.error(`Error deleting from sessionStorage for key '${key}':`, error);
    return false;
  }
}

export { SaveValueByKey, ReadValueByKey, RemoveValueByKey };

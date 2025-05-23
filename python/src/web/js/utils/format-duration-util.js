/**
 * Formats a duration in seconds into a human-readable string.
 * 
 * @param {number} seconds - Total duration in seconds.
 * @returns {string} Formatted time string: "mm:ss" or "hh:mm:ss".
 */
export const formatDuration = (seconds) => {
    /**
     * Pads a number with leading zeros to ensure it has at least two digits.
     * 
     * @param {number} num - The number to pad.
     * @returns {string} The padded number as a string.
     */
    const pad = (num) => num.toString().padStart(2, "0")

    const totalSeconds = Math.floor(seconds)
    const hours = Math.floor(totalSeconds / 3600)
    const minutes = Math.floor((totalSeconds % 3600) / 60)
    const secs = totalSeconds % 60

    if (hours > 0) {
        // hh:mm:ss
        return `${hours.toString()}:${pad(minutes)}:${pad(secs)}`
    } else {
        // mm:ss
        return `${minutes.toString()}:${pad(secs)}`
    }
}
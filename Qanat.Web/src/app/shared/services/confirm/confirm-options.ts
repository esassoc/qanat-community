import { IconInterface } from "src/app/shared/components/icon/icon.component";

/**
 * Options passed when opening a confirmation modal
 */
export interface ConfirmOptions {
    /**
     * The title of the confirmation modal
     */
    title: string;

    /**
     * The message in the confirmation modal
     */
    message: string;

    buttonClassYes: string;
    buttonTextYes: string;
    buttonTextNo: string;
    icon?: typeof IconInterface;
    iconColor?: string;
}

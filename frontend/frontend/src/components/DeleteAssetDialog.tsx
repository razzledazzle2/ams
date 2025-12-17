import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";
import { apiFetch } from "@/lib/apiFetch";
import { getAccessToken } from "@/utils/auth";
type Props = {
  asset: any;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onDeleted: () => void;
};
const API_BASE = "http://localhost:5051";

export function DeleteAssetDialog({
  asset,
  open,
  onOpenChange,
  onDeleted,
}: Props) {
  const handleDelete = async () => {
    try {
      const res = await apiFetch(`${API_BASE}/api/assets/${asset.id}`, {
        method: "DELETE",
      });

      if (!res.ok) throw new Error("Failed to delete asset");

      onDeleted();
      onOpenChange(false);
    } catch (err) {
      console.error(err);
    }
  };

  return (
    <AlertDialog open={open} onOpenChange={onOpenChange}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Are you absolutely sure?</AlertDialogTitle>
          <AlertDialogDescription>
            This action cannot be undone. This will permanently delete this asset.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel>Cancel</AlertDialogCancel>
          <AlertDialogAction className="bg-red-500 hover:bg-red-900" onClick={handleDelete}>Continue</AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}

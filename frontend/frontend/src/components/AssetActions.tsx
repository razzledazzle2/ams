import { Button } from "@/components/ui/button";
import { useState } from "react";
import { EditAssetDialog } from "./EditAssetDialog";
import { DeleteAssetDialog } from "./DeleteAssetDialog";
import { Pencil, Trash } from "lucide-react";

export const AssetActions = ({ asset, onUpdated }) => {
  const [editOpen, setEditOpen] = useState(false);
  const [deleteOpen, setDeleteOpen] = useState(false);
  return (
    <div>
      <Button variant="ghost" size="icon" onClick={() => setEditOpen(true)}>
        <Pencil />
      </Button>
      <Button
        variant="ghost"
        size="icon"
        className="    text-red-600 "
        onClick={() => setDeleteOpen(true)}
      >
        <Trash />
      </Button>
      <EditAssetDialog
        asset={asset}
        open={editOpen}
        onOpenChange={setEditOpen}
        onAssetUpdated={onUpdated}
      />

      {/* Delete modal */}
      <DeleteAssetDialog
        asset={asset}
        open={deleteOpen}
        onOpenChange={setDeleteOpen}
        onDeleted={onUpdated}
      />
    </div>
  );
};

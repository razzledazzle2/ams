
import { Button } from "@/components/ui/button";
import { useState } from "react";
import { EditAssetDialog } from "./EditAssetDialog";
import { DeleteAssetDialog } from "./DeleteAssetDialog";
export const AssetActions = ({asset, onUpdated}) => {
  const [editOpen, setEditOpen] = useState(false);
  const [deleteOpen, setDeleteOpen] = useState(false);
  return (
    <div>
      <Button onClick={() => setEditOpen(true)}>Edit</Button>
      <Button onClick={() => setDeleteOpen(true)}>Delete</Button>
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
  )
}
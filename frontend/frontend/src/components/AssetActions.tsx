import { Button } from "@/components/ui/button";
import { useState } from "react";
import { EditAssetDialog } from "./EditAssetDialog";
import { DeleteAssetDialog } from "./DeleteAssetDialog";
import { Pencil, Trash } from "lucide-react";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/components/ui/tooltip";

export const AssetActions = ({ asset, onUpdated }) => {
  const [editOpen, setEditOpen] = useState(false);
  const [deleteOpen, setDeleteOpen] = useState(false);

  return (
    <TooltipProvider>
      <div className="flex items-center gap-1">
        {/* Edit */}
        <Tooltip>
          <TooltipTrigger asChild>
            <Button
              variant="ghost"
              size="icon"
              onClick={() => setEditOpen(true)}
            >
              <Pencil />
            </Button>
          </TooltipTrigger>
          <TooltipContent>Edit asset</TooltipContent>
        </Tooltip>

        {/* Delete */}
        <Tooltip>
          <TooltipTrigger asChild>
            <Button
              variant="ghost"
              size="icon"
              className="text-red-600"
              onClick={() => setDeleteOpen(true)}
            >
              <Trash />
            </Button>
          </TooltipTrigger>
          <TooltipContent>Delete asset</TooltipContent>
        </Tooltip>

        {/* Edit modal */}
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
    </TooltipProvider>
  );
};
